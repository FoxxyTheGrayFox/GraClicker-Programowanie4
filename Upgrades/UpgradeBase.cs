using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjektClicker
{
    public abstract class UpgradeBase : IUpgrade
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract long Cost { get; protected set; }
        public abstract int Count { get; protected set; }
        public abstract bool CanBuy(GameState state);
        public abstract void Apply(GameState state);
        public abstract void IncreaseCost();
        public abstract void LoadFrom(UpgradeSaveData saved, GameState state);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}