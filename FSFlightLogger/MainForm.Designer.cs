namespace FSFlightLogger
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.OutputGroupbox = new System.Windows.Forms.GroupBox();
            this.StatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.FlightSimulatorStatusIcon = new System.Windows.Forms.PictureBox();
            this.FlightSimulatorLabel = new System.Windows.Forms.Label();
            this.FlightSimulatorStatusLabel = new System.Windows.Forms.Label();
            this.RecordButton = new System.Windows.Forms.Button();
            this.RecordingStatusIcon = new System.Windows.Forms.PictureBox();
            this.OutputCSVCheckbox = new System.Windows.Forms.CheckBox();
            this.OutputKMLCheckbox = new System.Windows.Forms.CheckBox();
            this.OutputCSVPathTextbox = new System.Windows.Forms.TextBox();
            this.OutputCSVPathLabel = new System.Windows.Forms.Label();
            this.OutputCSVPathBrowseButton = new System.Windows.Forms.Button();
            this.OutputKMLPathBrowseButton = new System.Windows.Forms.Button();
            this.OutputKMLPathLabel = new System.Windows.Forms.Label();
            this.OutputKMLPathTextbox = new System.Windows.Forms.TextBox();
            this.TriggersGroupbox = new System.Windows.Forms.GroupBox();
            this.TriggerWaitForMovementCheckbox = new System.Windows.Forms.CheckBox();
            this.TriggerNewLogStationaryCheckbox = new System.Windows.Forms.CheckBox();
            this.TriggerNewLogStationaryTimeEdit = new System.Windows.Forms.NumericUpDown();
            this.TriggerNewLogStationaryUnitsLabel = new System.Windows.Forms.Label();
            this.OutputGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FlightSimulatorStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecordingStatusIcon)).BeginInit();
            this.TriggersGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TriggerNewLogStationaryTimeEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // OutputGroupbox
            // 
            this.OutputGroupbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputGroupbox.Controls.Add(this.OutputKMLPathBrowseButton);
            this.OutputGroupbox.Controls.Add(this.OutputKMLPathLabel);
            this.OutputGroupbox.Controls.Add(this.OutputKMLPathTextbox);
            this.OutputGroupbox.Controls.Add(this.OutputCSVPathBrowseButton);
            this.OutputGroupbox.Controls.Add(this.OutputCSVPathLabel);
            this.OutputGroupbox.Controls.Add(this.OutputCSVPathTextbox);
            this.OutputGroupbox.Controls.Add(this.OutputKMLCheckbox);
            this.OutputGroupbox.Controls.Add(this.OutputCSVCheckbox);
            this.OutputGroupbox.Location = new System.Drawing.Point(12, 59);
            this.OutputGroupbox.Name = "OutputGroupbox";
            this.OutputGroupbox.Size = new System.Drawing.Size(482, 136);
            this.OutputGroupbox.TabIndex = 0;
            this.OutputGroupbox.TabStop = false;
            this.OutputGroupbox.Text = " Output ";
            // 
            // StatusImageList
            // 
            this.StatusImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StatusImageList.ImageStream")));
            this.StatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.StatusImageList.Images.SetKeyName(0, "Idle");
            this.StatusImageList.Images.SetKeyName(1, "Recording");
            this.StatusImageList.Images.SetKeyName(2, "FSDisconnected");
            this.StatusImageList.Images.SetKeyName(3, "FSConnected");
            // 
            // FlightSimulatorStatusIcon
            // 
            this.FlightSimulatorStatusIcon.Location = new System.Drawing.Point(12, 12);
            this.FlightSimulatorStatusIcon.Name = "FlightSimulatorStatusIcon";
            this.FlightSimulatorStatusIcon.Size = new System.Drawing.Size(32, 32);
            this.FlightSimulatorStatusIcon.TabIndex = 1;
            this.FlightSimulatorStatusIcon.TabStop = false;
            // 
            // FlightSimulatorLabel
            // 
            this.FlightSimulatorLabel.AutoSize = true;
            this.FlightSimulatorLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlightSimulatorLabel.Location = new System.Drawing.Point(50, 9);
            this.FlightSimulatorLabel.Name = "FlightSimulatorLabel";
            this.FlightSimulatorLabel.Size = new System.Drawing.Size(133, 21);
            this.FlightSimulatorLabel.TabIndex = 2;
            this.FlightSimulatorLabel.Text = "Flight Simulator";
            // 
            // FlightSimulatorStatusLabel
            // 
            this.FlightSimulatorStatusLabel.AutoSize = true;
            this.FlightSimulatorStatusLabel.Location = new System.Drawing.Point(51, 31);
            this.FlightSimulatorStatusLabel.Name = "FlightSimulatorStatusLabel";
            this.FlightSimulatorStatusLabel.Size = new System.Drawing.Size(70, 13);
            this.FlightSimulatorStatusLabel.TabIndex = 3;
            this.FlightSimulatorStatusLabel.Text = "Connecting...";
            // 
            // RecordButton
            // 
            this.RecordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RecordButton.Enabled = false;
            this.RecordButton.Location = new System.Drawing.Point(348, 17);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(108, 23);
            this.RecordButton.TabIndex = 4;
            this.RecordButton.Text = "&Start recording";
            this.RecordButton.UseVisualStyleBackColor = true;
            // 
            // RecordingStatusIcon
            // 
            this.RecordingStatusIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RecordingStatusIcon.Location = new System.Drawing.Point(462, 12);
            this.RecordingStatusIcon.Name = "RecordingStatusIcon";
            this.RecordingStatusIcon.Size = new System.Drawing.Size(32, 32);
            this.RecordingStatusIcon.TabIndex = 5;
            this.RecordingStatusIcon.TabStop = false;
            // 
            // OutputCSVCheckbox
            // 
            this.OutputCSVCheckbox.AutoSize = true;
            this.OutputCSVCheckbox.Location = new System.Drawing.Point(10, 23);
            this.OutputCSVCheckbox.Name = "OutputCSVCheckbox";
            this.OutputCSVCheckbox.Size = new System.Drawing.Size(141, 17);
            this.OutputCSVCheckbox.TabIndex = 0;
            this.OutputCSVCheckbox.Text = "Comma-separated (CSV)";
            this.OutputCSVCheckbox.UseVisualStyleBackColor = true;
            // 
            // OutputKMLCheckbox
            // 
            this.OutputKMLCheckbox.AutoSize = true;
            this.OutputKMLCheckbox.Location = new System.Drawing.Point(10, 79);
            this.OutputKMLCheckbox.Name = "OutputKMLCheckbox";
            this.OutputKMLCheckbox.Size = new System.Drawing.Size(119, 17);
            this.OutputKMLCheckbox.TabIndex = 1;
            this.OutputKMLCheckbox.Text = "Google Earth (KML)";
            this.OutputKMLCheckbox.UseVisualStyleBackColor = true;
            // 
            // OutputCSVPathTextbox
            // 
            this.OutputCSVPathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputCSVPathTextbox.Enabled = false;
            this.OutputCSVPathTextbox.Location = new System.Drawing.Point(71, 46);
            this.OutputCSVPathTextbox.Name = "OutputCSVPathTextbox";
            this.OutputCSVPathTextbox.Size = new System.Drawing.Size(373, 20);
            this.OutputCSVPathTextbox.TabIndex = 2;
            // 
            // OutputCSVPathLabel
            // 
            this.OutputCSVPathLabel.AutoSize = true;
            this.OutputCSVPathLabel.Location = new System.Drawing.Point(26, 49);
            this.OutputCSVPathLabel.Name = "OutputCSVPathLabel";
            this.OutputCSVPathLabel.Size = new System.Drawing.Size(32, 13);
            this.OutputCSVPathLabel.TabIndex = 3;
            this.OutputCSVPathLabel.Text = "Path:";
            // 
            // OutputCSVPathBrowseButton
            // 
            this.OutputCSVPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputCSVPathBrowseButton.Enabled = false;
            this.OutputCSVPathBrowseButton.Location = new System.Drawing.Point(448, 46);
            this.OutputCSVPathBrowseButton.Name = "OutputCSVPathBrowseButton";
            this.OutputCSVPathBrowseButton.Size = new System.Drawing.Size(28, 20);
            this.OutputCSVPathBrowseButton.TabIndex = 4;
            this.OutputCSVPathBrowseButton.Text = "...";
            this.OutputCSVPathBrowseButton.UseVisualStyleBackColor = true;
            // 
            // OutputKMLPathBrowseButton
            // 
            this.OutputKMLPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputKMLPathBrowseButton.Enabled = false;
            this.OutputKMLPathBrowseButton.Location = new System.Drawing.Point(448, 102);
            this.OutputKMLPathBrowseButton.Name = "OutputKMLPathBrowseButton";
            this.OutputKMLPathBrowseButton.Size = new System.Drawing.Size(28, 20);
            this.OutputKMLPathBrowseButton.TabIndex = 7;
            this.OutputKMLPathBrowseButton.Text = "...";
            this.OutputKMLPathBrowseButton.UseVisualStyleBackColor = true;
            // 
            // OutputKMLPathLabel
            // 
            this.OutputKMLPathLabel.AutoSize = true;
            this.OutputKMLPathLabel.Location = new System.Drawing.Point(26, 105);
            this.OutputKMLPathLabel.Name = "OutputKMLPathLabel";
            this.OutputKMLPathLabel.Size = new System.Drawing.Size(32, 13);
            this.OutputKMLPathLabel.TabIndex = 6;
            this.OutputKMLPathLabel.Text = "Path:";
            // 
            // OutputKMLPathTextbox
            // 
            this.OutputKMLPathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputKMLPathTextbox.Enabled = false;
            this.OutputKMLPathTextbox.Location = new System.Drawing.Point(71, 102);
            this.OutputKMLPathTextbox.Name = "OutputKMLPathTextbox";
            this.OutputKMLPathTextbox.Size = new System.Drawing.Size(373, 20);
            this.OutputKMLPathTextbox.TabIndex = 5;
            // 
            // TriggersGroupbox
            // 
            this.TriggersGroupbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TriggersGroupbox.Controls.Add(this.TriggerNewLogStationaryUnitsLabel);
            this.TriggersGroupbox.Controls.Add(this.TriggerNewLogStationaryTimeEdit);
            this.TriggersGroupbox.Controls.Add(this.TriggerNewLogStationaryCheckbox);
            this.TriggersGroupbox.Controls.Add(this.TriggerWaitForMovementCheckbox);
            this.TriggersGroupbox.Location = new System.Drawing.Point(12, 201);
            this.TriggersGroupbox.Name = "TriggersGroupbox";
            this.TriggersGroupbox.Size = new System.Drawing.Size(482, 99);
            this.TriggersGroupbox.TabIndex = 6;
            this.TriggersGroupbox.TabStop = false;
            this.TriggersGroupbox.Text = " Triggers ";
            // 
            // TriggerWaitForMovementCheckbox
            // 
            this.TriggerWaitForMovementCheckbox.AutoSize = true;
            this.TriggerWaitForMovementCheckbox.Checked = true;
            this.TriggerWaitForMovementCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TriggerWaitForMovementCheckbox.Location = new System.Drawing.Point(10, 23);
            this.TriggerWaitForMovementCheckbox.Name = "TriggerWaitForMovementCheckbox";
            this.TriggerWaitForMovementCheckbox.Size = new System.Drawing.Size(454, 17);
            this.TriggerWaitForMovementCheckbox.TabIndex = 0;
            this.TriggerWaitForMovementCheckbox.Text = " Wait for movement before logging the starting point (recommended, ignores initia" +
    "l teleports)";
            this.TriggerWaitForMovementCheckbox.UseVisualStyleBackColor = true;
            // 
            // TriggerNewLogStationaryCheckbox
            // 
            this.TriggerNewLogStationaryCheckbox.AutoSize = true;
            this.TriggerNewLogStationaryCheckbox.Location = new System.Drawing.Point(10, 46);
            this.TriggerNewLogStationaryCheckbox.Name = "TriggerNewLogStationaryCheckbox";
            this.TriggerNewLogStationaryCheckbox.Size = new System.Drawing.Size(353, 17);
            this.TriggerNewLogStationaryCheckbox.TabIndex = 1;
            this.TriggerNewLogStationaryCheckbox.Text = " Start a new log when stationary for at least (excluding when paused):";
            this.TriggerNewLogStationaryCheckbox.UseVisualStyleBackColor = true;
            // 
            // TriggerNewLogStationaryTimeEdit
            // 
            this.TriggerNewLogStationaryTimeEdit.Enabled = false;
            this.TriggerNewLogStationaryTimeEdit.Location = new System.Drawing.Point(33, 69);
            this.TriggerNewLogStationaryTimeEdit.Name = "TriggerNewLogStationaryTimeEdit";
            this.TriggerNewLogStationaryTimeEdit.Size = new System.Drawing.Size(78, 20);
            this.TriggerNewLogStationaryTimeEdit.TabIndex = 2;
            this.TriggerNewLogStationaryTimeEdit.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // TriggerNewLogStationaryUnitsLabel
            // 
            this.TriggerNewLogStationaryUnitsLabel.AutoSize = true;
            this.TriggerNewLogStationaryUnitsLabel.Location = new System.Drawing.Point(117, 71);
            this.TriggerNewLogStationaryUnitsLabel.Name = "TriggerNewLogStationaryUnitsLabel";
            this.TriggerNewLogStationaryUnitsLabel.Size = new System.Drawing.Size(47, 13);
            this.TriggerNewLogStationaryUnitsLabel.TabIndex = 3;
            this.TriggerNewLogStationaryUnitsLabel.Text = "seconds";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 309);
            this.Controls.Add(this.TriggersGroupbox);
            this.Controls.Add(this.RecordingStatusIcon);
            this.Controls.Add(this.RecordButton);
            this.Controls.Add(this.FlightSimulatorStatusLabel);
            this.Controls.Add(this.FlightSimulatorLabel);
            this.Controls.Add(this.FlightSimulatorStatusIcon);
            this.Controls.Add(this.OutputGroupbox);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FS Flight Logger";
            this.OutputGroupbox.ResumeLayout(false);
            this.OutputGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FlightSimulatorStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecordingStatusIcon)).EndInit();
            this.TriggersGroupbox.ResumeLayout(false);
            this.TriggersGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TriggerNewLogStationaryTimeEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox OutputGroupbox;
        private System.Windows.Forms.Button OutputKMLPathBrowseButton;
        private System.Windows.Forms.Label OutputKMLPathLabel;
        private System.Windows.Forms.TextBox OutputKMLPathTextbox;
        private System.Windows.Forms.Button OutputCSVPathBrowseButton;
        private System.Windows.Forms.Label OutputCSVPathLabel;
        private System.Windows.Forms.TextBox OutputCSVPathTextbox;
        private System.Windows.Forms.CheckBox OutputKMLCheckbox;
        private System.Windows.Forms.CheckBox OutputCSVCheckbox;
        private System.Windows.Forms.ImageList StatusImageList;
        private System.Windows.Forms.PictureBox FlightSimulatorStatusIcon;
        private System.Windows.Forms.Label FlightSimulatorLabel;
        private System.Windows.Forms.Label FlightSimulatorStatusLabel;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.PictureBox RecordingStatusIcon;
        private System.Windows.Forms.GroupBox TriggersGroupbox;
        private System.Windows.Forms.Label TriggerNewLogStationaryUnitsLabel;
        private System.Windows.Forms.NumericUpDown TriggerNewLogStationaryTimeEdit;
        private System.Windows.Forms.CheckBox TriggerNewLogStationaryCheckbox;
        private System.Windows.Forms.CheckBox TriggerWaitForMovementCheckbox;
    }
}

