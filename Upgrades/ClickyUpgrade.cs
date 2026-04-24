namespace ProjektClicker
{
    public enum ClickyStyle { Normal, Hat, Monocle }

    public class ClickyUpgrade : UpgradeBase
    {
        private bool _owned;
        private ClickyStyle _style = ClickyStyle.Normal;
        private long _cost = 200;
        private int _count;

        public override string Name => "Clicky";
        public override string Description => "Rozmowny pomocnik. Kliknięcia x1.5.";

        public override long Cost
        {
            get => _cost;
            protected set => SetField(ref _cost, value);
        }

        public override int Count
        {
            get => _count;
            protected set => SetField(ref _count, value);
        }

        public bool Owned
        {
            get => _owned;
            private set => SetField(ref _owned, value);
        }

        public ClickyStyle Style
        {
            get => _style;
            set => SetField(ref _style, value);
        }

        public override bool CanBuy(GameState state) => !Owned && state.Points >= Cost;

        public override void Apply(GameState state)
        {
            Owned = true;
            Count = 1;
            Style = ClickyStyle.Normal;
            state.SetBaseMultiplier(1.5);
        }

        public override void IncreaseCost() { }

        public override void LoadFrom(UpgradeSaveData saved, GameState state)
        {
            if (!saved.Owned) return;
            Apply(state);
            Style = saved.Style ?? ClickyStyle.Normal;
        }
    }
}