using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.chat_logs;

public class ChatLogsOptions : Options {
    private SaveData? _saveData;
    
    protected override string Name => "chat_logs";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<Message> Logs => _saveData!.Logs;

    
    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public void AddLog(Message message) {
        Logs.Add(message);
        Save();
    }
    
    public List<Message> GetLogs() {
        return Logs;
    }
}