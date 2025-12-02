using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.demon_list.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.demon_list;

public class DemonListOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "demon_list";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    

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
}