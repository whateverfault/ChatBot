using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.emotes.data;
using ChatBot.bot.services.emotes.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.emotes;

public class EmotesOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "emotes";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public bool Use7Tv => _saveData!.Use7Tv;
    public IReadOnlyDictionary<EmoteId, Emote> Emotes => _saveData!.Emotes;
    
    
    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public void SetUse7Tv(bool value) {
        _saveData!.Use7Tv = value;
        Save();
    }

    public bool GetUse7Tv() {
        return Use7Tv;
    }
}