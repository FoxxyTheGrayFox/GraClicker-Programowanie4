using System.IO;
using System.Text.Json;

namespace ProjektClicker
{
    public class SaveLoadService
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };
        public SaveLoadService()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var saveDir = Path.Combine(documents, "ClickerGame"); // miejsce zapisu - w dokumentach usera w folderze ClickerGame
            Directory.CreateDirectory(saveDir);
            _filePath = Path.Combine(saveDir, "save.json");
        }
        // --SAVE--
        public async Task SaveAsync(GameState state, IEnumerable<IUpgrade> upgrades)
        {
            var data = new SaveData
            {
                Points = state.Points,
                BaseMultiplier = state.BaseMultiplier,
                BoostMultiplier = state.BoostMultiplier,
                Upgrades = [.. upgrades.Select(MapUpgrade)]
            };
            var json = JsonSerializer.Serialize(data, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        // --LOAD--
        public async Task<SaveData?> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return null;
            
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<SaveData>(json);
        }
        // --MAPPING--
        private UpgradeSaveData MapUpgrade(IUpgrade upgrade)
        {
            return upgrade switch
            {
                ClickyUpgrade c => new UpgradeSaveData
                {
                    Name = c.Name,
                    Cost = c.Cost,
                    Count = c.Count,
                    Owned = c.Owned,
                    Style = c.Style
                },
                _ => new UpgradeSaveData
                {
                    Name = upgrade.Name,
                    Cost = upgrade.Cost,
                    Count = upgrade.Count
                }
            };
        }
    }
}
