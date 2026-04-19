namespace ProjektClicker
{
    public enum ClickyStyle
    {
        Normal,
        Hat,
        Monocle
    }
    public class ClickyUpgrade : IUpgrade
    {
        public string Name => "Clicky";
        public string Description => "Rozmowny pomocnik. Kliknięcia x1.5.";
        public long Cost { get; private set; } = 200;
        public int Count => Owned ? 1 : 0;
        public bool Owned { get; private set; }
        public ClickyStyle Style { get; set; } = ClickyStyle.Normal;
        public bool CanBuy(GameState state) => !Owned && state.Points >= Cost;

        public void Apply(GameState state)
        {
            Owned = true;
            state.SetBaseMultiplier(1.5);
            Style = ClickyStyle.Normal;
        }
        public void IncreaseCost() { }
    }
}