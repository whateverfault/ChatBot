using ChatBot.api.json;
using ChatBot.api.twitch.client;
using ChatBot.api.twitch.client.credentials;
using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.bot;

public class ChatBotOptions : Options {
    private static readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    protected override string Name => "chat_bot";
    protected override string OptionsPath => Path.Combine($"{Directories.DataDirectory}/{Name}", $"{Name}_opt.json");

    public ITwitchClient? Client { get; private set; }
    
    public override State ServiceState => State.Enabled;

    public ConnectionCredentials Credentials => _saveData!.Credentials;
    

    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.DataDirectory, Name), _saveData);
        }
    }

    public override void SetState(State state) {}

    public override State GetState() {
        return ServiceState;
    }

    public void UpdateClient(ITwitchClient client) {
        Client = client;
    }

    public void UpdateCredentials() {
        if (Client?.Credentials == null) return;
        SetCredentials(Client.Credentials);
    }
    
    public void SetCredentials(ConnectionCredentials credentials) {
        _saveData!.Credentials = credentials;
        Save();
    }
    
    public void SetCredentials(FullCredentials credentials) {
        _saveData!.Credentials = ConnectionCredentials.FromFullCredentials(credentials);
        Save();
    }
}