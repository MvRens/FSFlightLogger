namespace FSFlightLogger
{
    public class Settings
    {
        public int? MainFormLeft { get; set; }
        public int? MainFormTop { get; set; }

        public bool CSVEnabled { get; set; }
        public string CSVPath { get; set; }
        public bool KMLEnabled { get; set; }
        public string KMLPath { get; set; }
        public bool KMLLiveEnabled { get; set; }
        public int KMLLivePort { get; set; }

        public bool TriggerConnected { get; set; }
        public bool TriggerWaitForMovement { get; set; }
        public bool TriggerNewLogStationaryEnabled { get; set; }
        public int TriggerNewLogStationarySeconds { get; set; }
    }
}
