namespace ProjektClicker
{
    public class ClickyHatUpgrade(ClickyUpgrade clicky) : UpgradeBase
    {
        private bool _owned;
        private long _cost = 100;
        private int _count;
        private readonly ClickyUpgrade _clicky = clicky;

        public override string Name => "Kapelusz";
        public override string Description => "Stylowy kapelusz. Kliknięcia x1.75.";

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

        public override bool CanBuy(GameState state) =>
            !_owned && _clicky.Owned && state.Points >= Cost;

        public override void Apply(GameState state)
        {
            _owned = true;
            Count = 1;
            state.SetBaseMultiplier(1.75);
            _clicky.Style = ClickyStyle.Hat;
        }

        public override void IncreaseCost() { }

        public override void LoadFrom(UpgradeSaveData saved, GameState state)
        {
            if (saved.Count > 0) Apply(state);
        }
    }

    public class ClickyMonocleUpgrade(ClickyUpgrade clicky) : UpgradeBase
    {
        private bool _owned;
        private long _cost = 100;
        private int _count;
        private readonly ClickyUpgrade _clicky = clicky;

        public override string Name => "Monokl";
        public override string Description => "Elegancki monokl. Kliknięcia x2.";

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

        public override bool CanBuy(GameState state) =>
            !_owned && _clicky.Owned && _clicky.Style == ClickyStyle.Hat && state.Points >= Cost;

        public override void Apply(GameState state)
        {
            _owned = true;
            Count = 1;
            state.SetBaseMultiplier(2.0);
            _clicky.Style = ClickyStyle.Monocle;
        }

        public override void IncreaseCost() { }

        public override void LoadFrom(UpgradeSaveData saved, GameState state)
        {
            if (saved.Count > 0) Apply(state);
        }
    }
}