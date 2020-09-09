using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FSFlightLogger.Resources;
using Newtonsoft.Json;
using SimConnect;

namespace FSFlightLogger
{
    public partial class MainForm : Form, ISimConnectClientObserver
    {
        private readonly ISimConnectClientFactory simConnectClientFactory;
        private readonly CancellationTokenSource tryConnectCancellationTokenSource = new CancellationTokenSource();

        private ISimConnectClient simConnectClient;
        private SimConnectLogger logger;


        private enum FlightSimulatorStateValue
        {
            Connecting,
            Connected,
            Disconnected,
            Failed
        }

        private enum RecordingStateValue
        {
            Started,
            Stopped
        }


        public MainForm(ISimConnectClientFactory simConnectClientFactory)
        {
            this.simConnectClientFactory = simConnectClientFactory;

            InitializeComponent();

            FlightSimulatorState = FlightSimulatorStateValue.Connecting;
            RecordingState = RecordingStateValue.Stopped;
            LoadSettings();

            TryConnect();
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SaveSettings();

                components?.Dispose();
                simConnectClient?.DisposeAsync();
            }

            base.Dispose(disposing);
        }


        private string GetSettingsFilename()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"FSFlightLogger", @"config.json");
        }


        private void LoadSettings()
        {
            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), i18n.DefaultFolderName);
            var settings = new Settings
            {
                CSVPath = defaultPath,

                KMLEnabled = true,
                KMLPath = defaultPath,

                KMLLivePort = 2020,

                TriggerWaitForMovement = true
            };

            var filename = GetSettingsFilename();
            if (File.Exists(filename))
                using (var streamReader = new StreamReader(filename, Encoding.UTF8))
                {
                    var serializer = new JsonSerializer();
                    serializer.Populate(streamReader, settings);
                }


            if (settings.MainFormLeft.HasValue && settings.MainFormTop.HasValue)
            {
                StartPosition = FormStartPosition.Manual;
                Left = settings.MainFormLeft.Value;
                Top = settings.MainFormTop.Value;

                // Check if the user did not unplug the screen that last contained the form
                var formRectangle = new Rectangle(Left, Top, Width, Height);
                if (!Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle)))
                    StartPosition = FormStartPosition.CenterScreen;
            }

            OutputCSVCheckbox.Checked = settings.CSVEnabled;
            OutputCSVPathTextbox.Text = settings.CSVPath;
            OutputKMLCheckbox.Checked = settings.KMLEnabled;
            OutputKMLPathTextbox.Text = settings.KMLPath;
            KMLLiveCheckbox.Checked = settings.KMLLiveEnabled;
            KMLLivePortEdit.Value = settings.KMLLivePort;

            TriggerConnectedCheckbox.Checked = settings.TriggerConnected;
            TriggerWaitForMovementCheckbox.Checked = settings.TriggerWaitForMovement;
            TriggerNewLogStationaryCheckbox.Checked = settings.TriggerNewLogStationaryEnabled;
            TriggerNewLogStationaryTimeEdit.Value = settings.TriggerNewLogStationarySeconds;

            UpdateKMLLiveLink();
        }


        private void SaveSettings()
        {
            var settings = new Settings
            {
                MainFormTop = Top,
                MainFormLeft = Left,

                CSVEnabled = OutputCSVCheckbox.Checked,
                CSVPath = OutputCSVPathTextbox.Text,
                KMLEnabled = OutputKMLCheckbox.Checked,
                KMLPath = OutputKMLPathTextbox.Text,
                KMLLiveEnabled = KMLLiveCheckbox.Checked,
                KMLLivePort = (int)KMLLivePortEdit.Value,

                TriggerConnected = TriggerConnectedCheckbox.Checked,
                TriggerWaitForMovement = TriggerWaitForMovementCheckbox.Checked,
                TriggerNewLogStationaryEnabled = TriggerNewLogStationaryCheckbox.Checked,
                TriggerNewLogStationarySeconds = (int)TriggerNewLogStationaryTimeEdit.Value
            };

            var filename = GetSettingsFilename();
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            using (var streamWriter = new StreamWriter(filename, false, Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(streamWriter, settings);
            }
        }



        private void TryConnect()
        {
            FlightSimulatorState = FlightSimulatorStateValue.Connecting;

            simConnectClientFactory.TryConnect(@"FS Flight Logger")
                .ContinueWith((task, state) =>
                {
                    if (task.Result == null)
                    {
                        FlightSimulatorState = FlightSimulatorStateValue.Failed;
                        TryConnectTimer.Enabled = true;
                    }
                    else
                    {
                        simConnectClient = task.Result;
                        simConnectClient.AttachObserver(this);

                        logger = new SimConnectLogger(simConnectClient);
                        if (TriggerConnectedCheckbox.Checked)
                            StartRecording();

                        FlightSimulatorState = FlightSimulatorStateValue.Connected;
                    }
                }, 
                null, tryConnectCancellationTokenSource.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        public void OnQuit()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnQuit));
                return;
            }

            FlightSimulatorState = FlightSimulatorStateValue.Disconnected;
            TryConnectTimer.Enabled = true;
        }


        private FlightSimulatorStateValue internalFlightSimulatorState;
        private FlightSimulatorStateValue FlightSimulatorState
        {
            get => internalFlightSimulatorState;
            set
            {
                if (value == internalFlightSimulatorState)
                    return;

                internalFlightSimulatorState = value;
                var isConnected = value == FlightSimulatorStateValue.Connected;

                FlightSimulatorStatusIcon.Image = StatusImageList.Images[isConnected ? @"FSConnected" : @"FSDisconnected"];

                switch (value)
                {
                    case FlightSimulatorStateValue.Connecting:
                        FlightSimulatorStatusLabel.Text = i18n.FlightSimulatorConnecting;
                        break;

                    case FlightSimulatorStateValue.Connected:
                        FlightSimulatorStatusLabel.Text = i18n.FlightSimulatorConnected;
                        break;

                    case FlightSimulatorStateValue.Disconnected:
                        FlightSimulatorStatusLabel.Text = string.Format(i18n.FlightSimulatorQuit, TryConnectTimer.Interval / 1000);
                        break;

                    case FlightSimulatorStateValue.Failed:
                        FlightSimulatorStatusLabel.Text = string.Format(i18n.FlightSimulatorFailed, TryConnectTimer.Interval / 1000);
                        break;
                }

                CheckCanRecord();

                if (!isConnected)
                    RecordingState = RecordingStateValue.Stopped;
            }
        }


        private RecordingStateValue internalRecordingState;
        private RecordingStateValue RecordingState
        {
            get => internalRecordingState;
            set
            {
                if (value == internalRecordingState) 
                    return;

                internalRecordingState = value;
                var isRecording = value == RecordingStateValue.Started;

                RecordingStatusIcon.Image = StatusImageList.Images[isRecording ? @"Recording" : @"Idle"];
                RecordButton.Text = isRecording ? i18n.RecordButtonStop : i18n.RecordButtonStart;

                if (!isRecording)
                    ChangesWhileRecordingLabel.Visible = false;
            }
        }


        private void StartRecording()
        {
            SaveSettings();

            Task.Run(async () =>
            {
                await logger.Start(new SimConnectLogger.Config
                {
                    CSVOutputPath = OutputCSVCheckbox.Checked ? OutputCSVPathTextbox.Text : null,
                    KMLOutputPath = OutputKMLCheckbox.Checked ? OutputKMLPathTextbox.Text : null,
                    KMLLivePort = KMLLiveCheckbox.Checked ? (int) KMLLivePortEdit.Value : (int?) null,

                    IntervalTime = TimeSpan.FromSeconds(1), // TODO configurable
                    IntervalDistance = 1, // TODO configurable

                    WaitForMovement = TriggerWaitForMovementCheckbox.Checked,
                    NewLogWhenIdleSeconds = TriggerNewLogStationaryCheckbox.Checked
                        ? TimeSpan.FromSeconds((double) TriggerNewLogStationaryTimeEdit.Value)
                        : (TimeSpan?) null
                });
            });

            RecordingState = RecordingStateValue.Started;
        }

        private void StopRecording()
        {
            Task.Run(async () => await logger.Stop());
            RecordingState = RecordingStateValue.Stopped;
        }


        private void UpdateKMLLiveLink()
        {
            KMLLiveLinkEdit.Text = $@"http://localhost:{KMLLivePortEdit.Value}/live";
        }


        private void CheckChangesWhileRecording()
        {
            if (RecordingState == RecordingStateValue.Started)
                ChangesWhileRecordingLabel.Visible = true;
        }


        private void CheckCanRecord()
        {
            RecordButton.Enabled = FlightSimulatorState == FlightSimulatorStateValue.Connected &&
                                   (OutputCSVCheckbox.Checked || OutputKMLCheckbox.Checked || KMLLiveCheckbox.Checked);
        }


        private void TryConnectTimer_Tick(object sender, EventArgs e)
        {
            TryConnectTimer.Enabled = false;
            TryConnect();
        }


        private void OutputCSVCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            OutputCSVPathTextbox.Enabled = OutputCSVCheckbox.Checked;
            OutputCSVPathBrowseButton.Enabled = OutputCSVCheckbox.Checked;
            CheckChangesWhileRecording();
            CheckCanRecord();
        }

        private void OutputKMLCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            OutputKMLPathTextbox.Enabled = OutputKMLCheckbox.Checked;
            OutputKMLPathBrowseButton.Enabled = OutputKMLCheckbox.Checked;
            CheckChangesWhileRecording();
            CheckCanRecord();
        }


        private void TriggerNewLogStationaryCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            TriggerNewLogStationaryTimeEdit.Enabled = TriggerNewLogStationaryCheckbox.Checked;
            CheckChangesWhileRecording();
        }

        private void KMLLiveEnabled_CheckedChanged(object sender, EventArgs e)
        {
            KMLLivePortEdit.Enabled = KMLLiveCheckbox.Checked;
            CheckChangesWhileRecording();
            CheckCanRecord();
        }

        private void SelectPath(Control pathTextBox)
        {
            FolderBrowserDialog.SelectedPath = pathTextBox.Text;
            if (FolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                pathTextBox.Text = FolderBrowserDialog.SelectedPath;
        }

        private void OutputCSVPathBrowseButton_Click(object sender, EventArgs e)
        {
            SelectPath(OutputCSVPathTextbox);
        }

        private void OutputKMLPathBrowseButton_Click(object sender, EventArgs e)
        {
            SelectPath(OutputKMLPathTextbox);
        }


        private void KMLLivePortEdit_ValueChanged(object sender, EventArgs e)
        {
            UpdateKMLLiveLink();
        }

        private void Configuration_Changed(object sender, EventArgs e)
        {
            CheckChangesWhileRecording();
        }


        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (RecordingState == RecordingStateValue.Started)
                StopRecording();
            else
                StartRecording();
        }
    }
}
