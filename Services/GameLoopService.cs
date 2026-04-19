using System.Windows.Threading;

namespace ProjektClicker
{
    public class GameLoopService
    {
        private readonly DispatcherTimer _timer;
        public event Action? OnTick;
        public GameLoopService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => OnTick?.Invoke();
        }
        public void Start() => _timer.Start();
    }
}
