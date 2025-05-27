using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot;

public class ChatBotOptions : Options {
    private SaveData? _saveData = new();

    protected override string Name => "chat_bot";
    protected override string OptionsPath => Path.Combine($"{Directories.dataDirectory}{Name}", $"{Name}_opt.json");

    public override State State => _saveData!.state;
    public string? Username => _saveData!.username;
    public string? OAuth => _saveData!.oAuth;
    public string? Channel => _saveData!.channel;
    public bool ShouldPrintTwitchLogs => _saveData!.shouldPrintTwitchLogs;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData!);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData(
                                 State.Disabled,
                                 "Empty",
                                 "Empty",
                                 "Empty",
                                 false
                                );
        Save();
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.dataDirectory, Name), _saveData);
    }

    public override void SetState(State state) {
        _saveData!.state = state;
        Save();
    }

    public override State GetState() {
        return State;
    }

    public void SetUsername(string username) {
        _saveData!.username = username;
    }
    
    public void SetChannel(string channel) {
        _saveData!.channel = channel;
    }
    
    public void SetOAuth(string token) {
        _saveData!.oAuth = token;
    }

    public void SetOAuthDynamic(dynamic token) {
        _saveData!.oAuth = token;
    }
    
    public string GetUsername() {
        return Username ?? "Empty";
    }
    
    public string GetChannel() {
        return Channel ?? "Empty";
    }
    
    public string GetOAuth() {
        return OAuth ?? "Empty";
    }
    
    public void SetLoggingStatus(bool status) {
        _saveData!.shouldPrintTwitchLogs = status;
    }
}