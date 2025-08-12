using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot.services.chat_logs;

public class ChatLogsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "chat_logs";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<Message> Logs => _saveData!.Logs;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
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

    public void AddLog(Message message) {
        Logs.Add(message);
        Save();
    }
    
    public List<Message> GetLogs() {
        return Logs;
    }
}