using System.Collections.ObjectModel;

namespace ProjektClicker
{
    public class GameLogic(GameState state)
    {
        private readonly GameState _state = state;
        public ObservableCollection<IUpgrade> Upgrades { get; } = [];
        public void Click() => _state.AddPoints((long)(1 * _state.TotalMultiplier)); 

        public void BuyUpgrade(IUpgrade upgrade)
        {
            if (!upgrade.CanBuy(_state)) return;

            if (_state.SpendPoints(upgrade.Cost))
            {
                upgrade.Apply(_state);
                upgrade.IncreaseCost();
            }
        }
        public void Tick()
        {
            long totalProduction = Upgrades
                .OfType<CursorUpgrade>()
                .Sum(u => u.GetProduction());
                
            if (totalProduction > 0)
                _state.AddPoints(totalProduction);
        }
    }
}