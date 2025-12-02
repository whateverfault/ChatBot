using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs.data;
using ChatBot.bot.services.chat_logs.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.chat_logs;

public class ChatLogsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "chat_logs";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    private List<Message> Logs => _saveData!.Logs;
    

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

    public void AddLog(Message message) {
        Logs.Add(message);
        if (Logs.Count > 0 && Logs.Count % 25 == 0) {
            Save();
        }
    }
    
    public List<Message> GetLogs() {
        return Logs;
    }
    
    public int GetLogsCount() {
        return Logs.Count;
    }
}