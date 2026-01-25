using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.bot_lifecycle.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.bot_lifecycle;

public class BotLifecycleOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "bot_lifecycle";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    private long DisconnectTimeout => _saveData!.DisconnectTimeout;
    

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

    public long GetDisconnectTimeout() {
        return DisconnectTimeout;
    }
    
    public void SetDisconnectTimeout(long value) {
        _saveData!.DisconnectTimeout = value;
        Save();
    }
}