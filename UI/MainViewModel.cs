using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProjektClicker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GameState _state = new();
        private readonly GameLogic _logic;
        private readonly RandomEventService _randomEvents;
        private readonly GameLoopService _gameLoop;
        private readonly ClickyFactService _factService = new();
        private readonly SaveLoadService _saveService = new();
        private readonly ClickyImageService _imageService = new();
        private readonly ThemeService _themeService = new();
        private readonly ClickyUpgrade _clicky;
        private readonly Random _rand = new();

        public ICommand ClickCommand { get; }
        public ICommand BuyUpgradeCommand { get; }
        public ICommand ClaimBoostCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand ApplyThemeCommand { get; }

        public ObservableCollection<IUpgrade> Upgrades => _logic.Upgrades;

        public string PointsText => $"Punkty: {_state.Points}";

        private BitmapImage? _clickyImage;
        public BitmapImage? ClickyImage
        {
            get => _clickyImage;
            private set => SetField(ref _clickyImage, value);
        }

        private string _clickyFact = "";
        public string ClickyFact
        {
            get => _clickyFact;
            private set => SetField(ref _clickyFact, value);
        }

        private string _boostText = "";
        public string BoostText
        {
            get => _boostText;
            private set => SetField(ref _boostText, value);
        }

        private bool _boostVisible;
        public bool BoostVisible
        {
            get => _boostVisible;
            private set => SetField(ref _boostVisible, value);
        }

        private bool _boostStarVisible;
        public bool BoostStarVisible
        {
            get => _boostStarVisible;
            private set => SetField(ref _boostStarVisible, value);
        }

        private double _boostStarX;
        public double BoostStarX
        {
            get => _boostStarX;
            private set => SetField(ref _boostStarX, value);
        }

        private double _boostStarY;
        public double BoostStarY
        {
            get => _boostStarY;
            private set => SetField(ref _boostStarY, value);
        }

        public IReadOnlyList<GameTheme> AvailableThemes { get; } =
            Enum.GetValues<GameTheme>().ToList();

        private GameTheme _selectedTheme = GameTheme.Default;
        public GameTheme SelectedTheme
        {
            get => _selectedTheme;
            set => SetField(ref _selectedTheme, value);
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetField(ref _statusMessage, value);
        }

        public MainViewModel()
        {
            _clicky = new ClickyUpgrade();

            _logic = new GameLogic(_state);
            _logic.Upgrades.Add(new CursorUpgrade("Podstawowy cursor", 30, 1));
            _logic.Upgrades.Add(new CursorUpgrade("Lepszy cursor", 60, 2));
            _logic.Upgrades.Add(_clicky);
            _logic.Upgrades.Add(new ClickyHatUpgrade(_clicky));
            _logic.Upgrades.Add(new ClickyMonocleUpgrade(_clicky));

            ClickCommand = new RelayCommand(OnClick);

            BuyUpgradeCommand = new RelayCommand<IUpgrade>(
                upgrade => _logic.BuyUpgrade(upgrade),
                upgrade => upgrade?.CanBuy(_state) ?? false);

            ClaimBoostCommand = new RelayCommand(OnClaimBoost);

            SaveCommand = new RelayCommand(OnSave);
            LoadCommand = new RelayCommand(async () => await LoadGameAsync());
            ApplyThemeCommand = new RelayCommand(OnApplyTheme);

            _state.OnPointsChanged += _ =>
            {
                OnPropertyChanged(nameof(PointsText));
                RaiseCanExecuteChanged();
            };

            _clicky.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(ClickyUpgrade.Owned) or nameof(ClickyUpgrade.Style))
                    ClickyImage = _imageService.GetImage(_clicky);
            };

            _randomEvents = new RandomEventService(_state);
            _randomEvents.OnBoostSpawned += (_, e) => ShowBoostStar(e.NormalizedX, e.NormalizedY);
            _randomEvents.OnBoostExpired += (_, _) => BoostStarVisible = false;
            _randomEvents.OnBoostChanged += (active, seconds) =>
            {
                BoostVisible = active;
                BoostText = active ? $"Boost x2: {seconds}s" : "";
            };

            _gameLoop = new GameLoopService();
            _gameLoop.OnTick += OnTick;
            _gameLoop.Start();

            _ = LoadGameAsync();
        }

        private void OnTick()
        {
            _logic.Tick();
            _randomEvents.Tick();

            if (_clicky.Owned && _rand.Next(20) == 0)
                ClickyFact = _factService.GetRandomFact();
        }

        private void OnClick() => _logic.Click();

        private void OnClaimBoost() => _randomEvents.ClaimBoost();

        private void ShowBoostStar(double normalizedX, double normalizedY)
        {
            BoostStarX = normalizedX * (1000 - 60);
            BoostStarY = normalizedY * (600 - 60);
            BoostStarVisible = true;
        }

        private void OnSave()
        {
            _ = _saveService.SaveAsync(_state, Upgrades, SelectedTheme);
            StatusMessage = $"Zapisano — {DateTime.Now:HH:mm:ss}";
        }

        private async Task LoadGameAsync()
        {
            var save = await _saveService.LoadAsync();
            if (save == null)
            {
                StatusMessage = "Brak zapisu — nowa gra";
                return;
            }

            _state.Load(save.Points, save.BaseMultiplier);

            foreach (var savedUpgrade in save.Upgrades)
            {
                var upgrade = Upgrades.FirstOrDefault(u => u.Name == savedUpgrade.Name);
                upgrade?.LoadFrom(savedUpgrade, _state);
            }

            SelectedTheme = save.Theme;
            OnApplyTheme();

            OnPropertyChanged(nameof(PointsText));
            RaiseCanExecuteChanged();
            StatusMessage = $"Wczytano — {DateTime.Now:HH:mm:ss}";
        }

        private void OnApplyTheme()
        {
            _themeService.Apply(SelectedTheme);
            StatusMessage = $"Motyw zmieniony na: {SelectedTheme}";
        }

        private void RaiseCanExecuteChanged()
        {
            if (BuyUpgradeCommand is RelayCommand<IUpgrade> cmd)
                cmd.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}