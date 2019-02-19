namespace EmptyIt
{
    [ConfigurationPath("EmptyItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public int Interval { get; set; } = 1;
        public bool EmptyLandfillSites { get; set; } = true;
        public int ThresholdLandfillSites { get; set; } = 75;
        public bool EmptyCemeteries { get; set; } = true;
        public int ThresholdCemeteries { get; set; } = 75;
        public bool EmptySnowDumps { get; set; } = true;
        public int ThresholdSnowDumps { get; set; } = 75;

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