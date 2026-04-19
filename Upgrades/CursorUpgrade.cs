namespace ProjektClicker
{
    public class CursorUpgrade(string name, long cost, int production) : IUpgrade
    {
        public string Name { get; } = name;
        public string Description => $"Generuje {_production} punkt(ów) na sekundę";
        public long Cost { get; private set; } = cost;
        public int Count { get; private set; }
        private readonly int _production = production;
        public bool CanBuy(GameState state) => state.Points >= Cost;
        public void Apply(GameState state) => Count++;
        public void SetCost(long cost) => Cost = cost;
        public long GetProduction() => Count * _production;
        public void IncreaseCost() => Cost = (long)(Cost * 1.1);
    }
}