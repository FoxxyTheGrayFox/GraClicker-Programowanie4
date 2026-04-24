namespace ProjektClicker
{
    public class SaveData
    {
        public long Points { get; set; }
        public double BaseMultiplier { get; set; }
        public List<UpgradeSaveData> Upgrades { get; set; } = [];
        public GameTheme Theme { get; set; } = GameTheme.Default;
    }

    public class UpgradeSaveData
    {
        public string Name { get; set; } = "";
        public long Cost { get; set; }
        public int Count { get; set; }
        public bool Owned { get; set; }
        public ClickyStyle? Style { get; set; }
    }
}