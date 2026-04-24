namespace ProjektClicker
{
    public class GameState
    {
        public long Points { get; private set; }
        public double BaseMultiplier { get; private set; } = 1;
        public double BoostMultiplier { get; private set; } = 1;
        public double TotalMultiplier => BaseMultiplier * BoostMultiplier;

        public event Action<long>? OnPointsChanged;

        public void AddPoints(long amount)
        {
            Points += amount;
            OnPointsChanged?.Invoke(Points);
        }

        public bool SpendPoints(long amount)
        {
            if (Points < amount) return false;
            Points -= amount;
            OnPointsChanged?.Invoke(Points);
            return true;
        }

        public void Load(long points, double baseMultiplier)
        {
            Points = points;
            BaseMultiplier = baseMultiplier;
            OnPointsChanged?.Invoke(Points);
        }

        public void SetBaseMultiplier(double multiplier) => BaseMultiplier = multiplier;
        public void SetBoostMultiplier(double multiplier) => BoostMultiplier = multiplier;
    }
}