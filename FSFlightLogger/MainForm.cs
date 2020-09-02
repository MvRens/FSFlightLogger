using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SimConnect;

namespace FSFlightLogger
{
    public partial class MainForm : Form
    {
        private readonly ISimConnectClientFactory simConnectClientFactory;


        public MainForm(ISimConnectClientFactory simConnectClientFactory)
        {
            this.simConnectClientFactory = simConnectClientFactory;

            InitializeComponent();

            SetFlightSimulatorConnected(false);
            SetRecording(false);
            //var simConnectClient = simConnectClientFactory.TryConnect("FS Flight Logger");
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }


        private void SetFlightSimulatorConnected(bool connected)
        {
            FlightSimulatorStatusIcon.Image = StatusImageList.Images[connected ? "FSConnected" : "FSDisconnected"];
        }

        private void SetRecording(bool recording)
        {
            RecordingStatusIcon.Image = StatusImageList.Images[recording ? "Recording" : "Idle"];
        }
    }
}
