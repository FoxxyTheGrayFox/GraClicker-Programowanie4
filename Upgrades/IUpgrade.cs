namespace ProjektClicker
{
    public interface IUpgrade
    {
        string Name { get; }
        string Description { get; } // tooltip
        long Cost { get; }
        int Count { get; }
        bool CanBuy(GameState state);
        void Apply(GameState state);
        void IncreaseCost();
    }
}