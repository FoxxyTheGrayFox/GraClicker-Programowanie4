
namespace ProjektClicker
{
    public class ClickyHatUpgrade(ClickyUpgrade clicky) : IUpgrade
    {
        public string Name => "Kapelusz";
        public string Description => "Stylowy kapelusz. Kliknięcia x1.75.";
        public long Cost { get; private set; } = 100;
        public int Count => _owned ? 1 : 0;
        private bool _owned;
        private readonly ClickyUpgrade _clicky = clicky;
        public bool CanBuy(GameState state) => !_owned && _clicky.Owned && state.Points >= Cost;

        public void Apply(GameState state)
        {
            _owned = true;
            state.SetBaseMultiplier(1.75);
            _clicky.Style = ClickyStyle.Hat;
        }
        public void IncreaseCost() { }
    }
    public class ClickyMonocleUpgrade(ClickyUpgrade clicky) : IUpgrade
    {
        public string Name => "Monokl";
        public string Description => "Elegancki monokl. Kliknięcia x2";
        public long Cost { get; private set; } = 100;
        public int Count => _owned ? 1 : 0;
        private bool _owned;
        private readonly ClickyUpgrade _clicky = clicky;
        public bool CanBuy(GameState state) =>
            !_owned && _clicky.Owned && _clicky.Style == ClickyStyle.Hat && state.Points >= Cost;

        public void Apply(GameState state)
        {
            _owned = true;
            state.SetBaseMultiplier(2.0);
            _clicky.Style = ClickyStyle.Monocle;
        }
        public void IncreaseCost() { }
    }
}