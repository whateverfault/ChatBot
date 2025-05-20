using ChatBot.Shared;
using ChatBot.Shared.interfaces;
using ChatBot.utils;

namespace ChatBot.twitchAPI;

public class ChatBotOptions : Options {
    public SaveData _saveData = new();
    
    protected override string Name => "chat_bot";
    protected override string OptionsPath => Path.Combine(Directories.dataDirectory, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData.state;
    public string? Username => _saveData.username;
    public string? OAuth => _saveData.oAuth;
    public string? Channel => _saveData.channel;
    public bool ShouldPrintTwitchLogs => _saveData.shouldPrintTwitchLogs;

    
    public override bool Load() {
        if (!Path.Exists(OptionsPath)) return false;
        JsonUtils.TryRead(OptionsPath, out _saveData!);
        return true;
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }
    
    public void ToDefaults() {
        _saveData.state = State.Disabled;
        _saveData.username = string.Empty;
        _saveData.oAuth = string.Empty;
        _saveData.channel = string.Empty;
        _saveData.shouldPrintTwitchLogs = false;
    }
    
    public void SetState(State state) {
        _saveData.state = state;
    }

    public void SetUsername(string username) {
        _saveData.username = username;
    }
    
    public void SetChannel(string channel) {
        _saveData.channel = channel;
    }

    public void SetOAuth(string token) {
        _saveData.oAuth = token;
    }
    
    public void SetLoggingStatus(bool status) {
        _saveData.shouldPrintTwitchLogs = status;
    }
}