namespace ProjektClicker
{
    public enum BoostPanel { Left, Center, Right }

    public class BoostSpawnedEventArgs(BoostPanel panel, int timeoutSeconds) : EventArgs
    {
        public BoostPanel Panel { get; } = panel;
        public int TimeoutSeconds { get; } = timeoutSeconds;
    }

    public class RandomEventService(GameState state)
    {
        private static readonly Random _random = new();
        private readonly GameState _state = state;

        private bool _boostActive = false;
        private bool _boostWaitingForClick = false;
        private int _remainingSeconds = 0;

        private const int BoostDurationSeconds = 30;
        private const int BoostTimeoutSeconds = 10; 
        private const int BoostChanceOneIn = 100;

        public event EventHandler<BoostSpawnedEventArgs>? OnBoostSpawned;
        public event EventHandler? OnBoostExpired;
        public event Action<bool, int>? OnBoostChanged;

        public void Tick()
        {
            if (_boostWaitingForClick)
            {
                _remainingSeconds--;
                if (_remainingSeconds <= 0)
                    ExpireBoost();
                return;
            }

            if (_boostActive)
            {
                _remainingSeconds--;
                OnBoostChanged?.Invoke(true, _remainingSeconds);
                if (_remainingSeconds <= 0)
                    EndBoost();
                return;
            }

            if (_random.Next(BoostChanceOneIn) == 0)
                SpawnBoost();
        }
        public class BoostSpawnedEventArgs(double normalizedX, double normalizedY, int timeoutSeconds) : EventArgs
        {
            public double NormalizedX { get; } = normalizedX; 
            public double NormalizedY { get; } = normalizedY;
            public int TimeoutSeconds { get; } = timeoutSeconds;
        }

        public void ClaimBoost()
        {
            if (!_boostWaitingForClick) return;

            _boostWaitingForClick = false;
            OnBoostExpired?.Invoke(this, EventArgs.Empty);
            StartBoost();
        }

        private void SpawnBoost()
        {
            _boostWaitingForClick = true;
            _remainingSeconds = BoostTimeoutSeconds;

            double x = 0.05 + _random.NextDouble() * 0.90;
            double y = 0.05 + _random.NextDouble() * 0.90;

            OnBoostSpawned?.Invoke(this, new BoostSpawnedEventArgs(x, y, BoostTimeoutSeconds));
        }

        private void ExpireBoost()
        {
            _boostWaitingForClick = false;
            OnBoostExpired?.Invoke(this, EventArgs.Empty);
        }

        private void StartBoost()
        {
            _boostActive = true;
            _remainingSeconds = BoostDurationSeconds;
            _state.SetBoostMultiplier(2);
            OnBoostChanged?.Invoke(true, _remainingSeconds);
        }

        private void EndBoost()
        {
            _boostActive = false;
            _state.SetBoostMultiplier(1);
            OnBoostChanged?.Invoke(false, 0);
        }

    }
}