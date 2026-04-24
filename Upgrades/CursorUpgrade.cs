namespace ProjektClicker
{
    public class CursorUpgrade(string name, long baseCost, int production) : UpgradeBase
    {
        private long _cost = baseCost;
        private int _count;
        private readonly int _production = production;

        public override string Name { get; } = name;
        public override string Description => $"Generuje {_production} punkt(ów) na sekundę";

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

        public override bool CanBuy(GameState state) => state.Points >= Cost;

        public override void Apply(GameState state) => Count++;

        public override void IncreaseCost() => Cost = (long)(Cost * 1.1);

        public long GetProduction() => Count * _production;

        public override void LoadFrom(UpgradeSaveData saved, GameState state)
        {
            for (int i = 0; i < saved.Count; i++)
                Apply(state);

            Cost = saved.Cost;
        }
    }
}