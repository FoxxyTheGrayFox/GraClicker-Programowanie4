namespace ProjektClicker
{
    public class RandomEventService(GameState state)
    {
        private static readonly Random _random = new();
        private readonly GameState _state = state;
        private bool _boostActive = false;
        private int _remainingSeconds = 0; // pozostały czas boostu
        public event Action<bool, int>? OnBoostChanged;

        public void Tick()
        {
            if (_boostActive)
            {
                _remainingSeconds--;
                OnBoostChanged?.Invoke(true, _remainingSeconds);
                if (_remainingSeconds <= 0) // koniec czasu
                    EndBoost();

                return;
            }
            if (_random.Next(0, 200) == 0) // 0.5%
                StartBoost();
        }
        private void StartBoost()
        {
            if (_boostActive)
                return;
                
            _boostActive = true;
            _remainingSeconds = 30; // domyślnie trwa 30s
            _state.SetBoostMultiplier(2); // mnożnik x2 
            OnBoostChanged?.Invoke(true, _remainingSeconds);
        }
        private void EndBoost()
        {
            _boostActive = false;
            _state.SetBoostMultiplier(1); // revert mnożnika
            OnBoostChanged?.Invoke(false, 0);
        }
    }
}