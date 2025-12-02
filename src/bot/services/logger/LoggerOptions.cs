using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.logger.data;
using ChatBot.bot.services.logger.data.saved;
using ChatBot.bot.shared;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger;

public class LoggerOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "logger";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    
    private List<Log> Logs => _saveData!.Logs;

    public LogLevel LogLevel => _saveData!.LogLevel; 
    

    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData)) {
            SetDefaults();
        }
    }

    public override void Save() {
        if (_saveData == null) {
            return;
        }
        
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

    public void SetLogLevel(int logLevel) {
        _saveData!.LogLevel = (LogLevel)logLevel;
        Save();
    }
    
    public void AddLog(Log log) {
        Logs.Add(log);
        Save();
    }
}