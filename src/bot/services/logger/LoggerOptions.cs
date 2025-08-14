using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;
using ChatBot.bot.utils;

namespace ChatBot.bot.services.logger;

public class LoggerOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    protected override string Name => "logger";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public List<Log> Logs => _saveData!.Logs;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        if (_saveData == null) {
            return;
        }
        
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), new SaveData(_saveData));
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.SetState(state);
        Save();
    }

    public void AddLog(Log log) {
        Logs.Add(log);
        Save();
    }
}