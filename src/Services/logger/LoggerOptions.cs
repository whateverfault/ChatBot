using System.Resources;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.logger;

public class LoggerOptions : Options {
    private SaveData? _saveData;
    protected override string Name => "logger";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public List<Log> Logs => _saveData!.Logs;
    public List<Log> TwitchLogs => _saveData!.TwitchLogs;

    
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
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData(State.Disabled);
        Save();
    }

    public override void SetState(State state) {
        _saveData!.SetState(state);
        Save();
    }

    public override State GetState() {
        return _saveData!.GetState();
    }

    public void AddLog(Log log) {
        Logs.Add(log);
        Save();
    }
    
    public void AddTwitchLog(Log log) {
        TwitchLogs.Add(log);
        Save();
    }
}