using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot;

public class ChatBotOptions : Options {
    private SaveData? _saveData = new();

    protected override string Name => "chat_bot";
    protected override string OptionsPath => Path.Combine($"{Directories.dataDirectory}{Name}", $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public string? Username => _saveData!.Username;
    public string? OAuth => _saveData!.OAuth;
    public string? Channel => _saveData!.Channel;


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
                                 "Empty"
                                 );
        Save();
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.dataDirectory, Name), _saveData);
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public void SetUsername(string username) {
        _saveData!.Username = username;
    }
    
    public void SetChannel(string channel) {
        _saveData!.Channel = channel;
    }
    
    public void SetOAuth(string token) {
        _saveData!.OAuth = token;
    }

    public void SetOAuthDynamic(dynamic token) {
        _saveData!.OAuth = token;
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
}