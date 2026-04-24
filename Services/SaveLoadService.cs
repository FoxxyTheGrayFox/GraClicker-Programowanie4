using System.IO;
using System.Text.Json;

namespace ProjektClicker
{
    public class SaveLoadService
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public SaveLoadService()
        {
            var saveDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ClickerGame");
            Directory.CreateDirectory(saveDir);
            _filePath = Path.Combine(saveDir, "save.json");
        }

        public async Task SaveAsync(GameState state, IEnumerable<IUpgrade> upgrades, GameTheme theme)
        {
            var data = new SaveData
            {
                Points = state.Points,
                BaseMultiplier = state.BaseMultiplier,
                Theme = theme,
                Upgrades = [.. upgrades.Select(MapToSaveData)]
            };
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(data, _options));
        }

        public async Task<SaveData?> LoadAsync()
        {
            if (!File.Exists(_filePath)) return null;
            return JsonSerializer.Deserialize<SaveData>(await File.ReadAllTextAsync(_filePath));
        }

        private static UpgradeSaveData MapToSaveData(IUpgrade upgrade) => upgrade switch
        {
            ClickyUpgrade c => new() { Name = c.Name, Cost = c.Cost, Count = c.Count, Owned = c.Owned, Style = c.Style },
            _               => new() { Name = upgrade.Name, Cost = upgrade.Cost, Count = upgrade.Count }
        };
    }
}