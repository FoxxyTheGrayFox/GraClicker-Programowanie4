using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
/*TODO:
Odświeżanie kosztu i ilości nie działa ale nie chce mi się tego poprawiać*/
namespace ProjektClicker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GameState _state = new();
        private readonly GameLogic _logic;
        private readonly Random _rand = new();
        private readonly RandomEventService _randomEvents;
        private readonly GameLoopService _gameLoop;
        private readonly ClickyFactService _factService = new();
        private readonly SaveLoadService _saveService = new();
        private readonly ClickyUpgrade _clicky;
        public ObservableCollection<IUpgrade> Upgrades => _logic.Upgrades;
        public ICommand ClickCommand { get; }
        public ICommand BuyUpgradeCommand { get; }
        public string PointsText => $"Punkty: {_state.Points}";
        private BitmapImage? _clickyImage;
        public BitmapImage? ClickyImage
        {
            get => _clickyImage;
            set => SetField(ref _clickyImage, value);
        }
        private string _clickyFact = "";
        public string ClickyFact
        {
            get => _clickyFact;
            set => SetField(ref _clickyFact, value);
        }
        private string _boostText = "";
        public string BoostText
        {
            get => _boostText;
            set => SetField(ref _boostText, value);
        }
        private bool _boostVisible;
        public bool BoostVisible
        {
            get => _boostVisible;
            set => SetField(ref _boostVisible, value);
        }
        private int _autoSaveCounter = 0;
        // --INIT--
        public MainViewModel()
        {
            _logic = new GameLogic(_state);
            _clicky = new ClickyUpgrade();
            _logic.Upgrades.Add(new CursorUpgrade("Podstawowy cursor", 30, 1));
            _logic.Upgrades.Add(new CursorUpgrade("Lepszy cursor", 60, 2));
            _logic.Upgrades.Add(_clicky);
            _logic.Upgrades.Add(new ClickyHatUpgrade(_clicky));
            _logic.Upgrades.Add(new ClickyMonocleUpgrade(_clicky));

            ClickCommand = new RelayCommand(OnClick);
            BuyUpgradeCommand = new RelayCommand<IUpgrade>(
                u =>
                {
                    _logic.BuyUpgrade(u);
                    RefreshUI();
                    RaiseCommands();
                },
                u => u != null && u.CanBuy(_state)
            );
            _state.OnPointsChanged += _ =>
            {
                RefreshUI();
                RaiseCommands();
            };
            _randomEvents = new RandomEventService(_state);
            _randomEvents.OnBoostChanged += (active, seconds) =>
            {
                BoostVisible = active;
                BoostText = active ? $"Boost x2: {seconds}s" : "";
            };
            _gameLoop = new GameLoopService();
            _gameLoop.OnTick += OnTick;
            _gameLoop.Start();
            // async save load
            _ = LoadGameAsync();
            RefreshUI();
        }
        // --GAMELOOP--
        private void OnTick()
        {
            _logic.Tick();
            _randomEvents.Tick();
            // 5% szansy na wyświetlenie faktu
            if (_clicky.Owned && _rand.Next(20) == 0)
            {
                ClickyFact = _factService.GetRandomFact();
            }
            RefreshUI();
            RaiseCommands();
            _autoSaveCounter++;
            if (_autoSaveCounter >= 5) // Autosave co 5s
            {
                _autoSaveCounter = 0;
                _ = _saveService.SaveAsync(_state, Upgrades);
            }
        }
        private void OnClick()
        {
            _logic.Click();
            RefreshUI();
            RaiseCommands();
        }
        // --SAVE MANAGEMENT THINGY--
        private async Task LoadGameAsync()
        {
            var save = await _saveService.LoadAsync();
            if (save != null)
            {
                ApplySaveData(save);
                RefreshUI();
                RaiseCommands();
            }
        }
        private void ApplySaveData(SaveData data)
        {
            _state.Load(data.Points, data.BaseMultiplier, data.BoostMultiplier);

            foreach (var saved in data.Upgrades)
            {
                var upgrade = Upgrades.FirstOrDefault(u => u.Name == saved.Name);
                if (upgrade == null) continue;
                switch (upgrade)
                {
                    case CursorUpgrade c:
                        for (int i = 0; i < saved.Count; i++)
                            c.Apply(_state);

                        c.SetCost(saved.Cost);
                        break;

                    case ClickyUpgrade clicky:
                        if (saved.Owned)
                        {
                            clicky.Apply(_state);
                            clicky.Style = saved.Style ?? ClickyStyle.Normal;
                        }
                        break;

                    case ClickyHatUpgrade hat:
                        if (saved.Count > 0)
                            hat.Apply(_state);

                        break;

                    case ClickyMonocleUpgrade mono:
                        if (saved.Count > 0)
                            mono.Apply(_state);
                            
                        break;
                }
            }
        }
        // --UI--
        private void RefreshUI()
        {
            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(Upgrades));
            ClickyImage = ClickyImageService.GetImage(_clicky);
        }
        private void RaiseCommands()
        {
            if (BuyUpgradeCommand is RelayCommand<IUpgrade> cmd)
                cmd.RaiseCanExecuteChanged();
        }
        // --PROPERTYCHANGEDHANDLER--
        public event PropertyChangedEventHandler? PropertyChanged;
        private void SetField<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            OnPropertyChanged(name);
        }
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}