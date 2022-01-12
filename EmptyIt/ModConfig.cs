namespace EmptyIt
{
    [ConfigurationPath("EmptyItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public int Interval { get; set; } = 1;
        public bool EmptyWarehouses { get; set; } = false;
        public int UpperThresholdWarehouses { get; set; } = 75;
        public bool StopEmptyingWarehouses { get; set; } = false;
        public int LowerThresholdWarehouses { get; set; } = 25;
        public bool EmptyLandfillSites { get; set; } = true;
        public int UpperThresholdLandfillSites { get; set; } = 75;
        public bool StopEmptyingLandfillSites { get; set; } = true;
        public int LowerThresholdLandfillSites { get; set; } = 25;
        public bool EmptyWasteTransferFacilities { get; set; } = false;
        public int UpperThresholdWasteTransferFacilities { get; set; } = 75;
        public bool StopEmptyingWasteTransferFacilities { get; set; } = false;
        public int LowerThresholdWasteTransferFacilities { get; set; } = 25;
        public bool EmptyCemeteries { get; set; } = true;
        public int UpperThresholdCemeteries { get; set; } = 75;
        public bool StopEmptyingCemeteries { get; set; } = true;
        public int LowerThresholdCemeteries { get; set; } = 25;
        public bool EmptySnowDumps { get; set; } = true;
        public int UpperThresholdSnowDumps { get; set; } = 75;
        public bool StopEmptyingSnowDumps { get; set; } = true;
        public int LowerThresholdSnowDumps { get; set; } = 25;

        private static ModConfig instance;

        public static ModConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Configuration<ModConfig>.Load();
                }

                return instance;
            }
        }

        public void Save()
        {
            Configuration<ModConfig>.Save();
            ConfigUpdated = true;
        }
    }
}