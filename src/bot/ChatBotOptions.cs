using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot;

public class ChatBotOptions : Options {
    private static readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    protected override string Name => "chat_bot";
    protected override string OptionsPath => Path.Combine($"{Directories.DataDirectory}/{Name}", $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public string? Username => _saveData!.Username;
    public string? OAuth => _saveData!.OAuth;
    public string? BroadcasterOAuth => _saveData!.BroadcasterOAuth;
    public string? Channel => _saveData!.Channel;
    public string? ClientId => _saveData!.ClientId;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.DataDirectory, Name), _saveData);
        }
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
        Save();
    }
    
    public void SetChannel(string channel) {
        _saveData!.Channel = channel;
        Save();
    }
    
    public void SetOAuth(string token) {
        _saveData!.OAuth = token;
        Save();
    }
    
    public void SetBroadcasterOAuth(string token) {
        _saveData!.BroadcasterOAuth = token;
        Save();
    }
    
    public void SetClientId(string clientId) {
        _saveData!.ClientId = clientId;
        Save();
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
    
    public string GetBroadcasterOAuth() {
        return BroadcasterOAuth ?? "Empty";
    }
    
    public string GetClientId() {
        return ClientId ?? "Empty";
    }
}