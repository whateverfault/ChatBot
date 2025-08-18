using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using Newtonsoft.Json;

namespace ChatBot.bot.services.presets;

public class Preset {
    private static readonly PresetsService _presets = (PresetsService)ServiceManager.GetService(ServiceName.Presets);

    [JsonProperty("name")]
    public string Name { get; private set; }

    [JsonProperty("id")]
    public int Id { get; private set; }


    [JsonConstructor]
    public Preset(
        [JsonProperty("name")] string name,
        [JsonProperty("id")] int id) {
        Name = name;
        Id = id;
    }

    public void Create() {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.ServicesToDefault([ServiceName.Presets,]);

        var bot = TwitchChatBot.Instance;
        bot.Options.SetDefaults();
        bot.Stop();
    }

    public void Load() {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.LoadServices([ServiceName.Presets,]);
        
        var bot = TwitchChatBot.Instance;
        bot.Options.Load();
        bot.Stop();
    }

    public string GetName() {
        return Name;
    }
    
    public void SetName(string name) {
        if (Name.Equals(name)) return;
        Directory.Move(Path.Combine(Directories.ConfigDirectory, $"{Name}_data"), Path.Combine(Directories.ConfigDirectory, $"{name}_data"));
        Name = name;

        if (_presets.Options.GetCurrentPreset() == Id) Directories.ChangeDataDirectory($"{name}_data");
        _presets.Options.Save();
    }

    public void ReassignId(int id) {
        Id = id;
    }
}