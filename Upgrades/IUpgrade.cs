using System.ComponentModel;

namespace ProjektClicker
{
    public interface IUpgrade : INotifyPropertyChanged
    {
        string Name { get; }
        string Description { get; }
        long Cost { get; }
        int Count { get; }
        bool CanBuy(GameState state);
        void Apply(GameState state);
        void IncreaseCost();
        void LoadFrom(UpgradeSaveData saved, GameState state);
    }
}