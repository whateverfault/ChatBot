using ChatBot.api.json;
using ChatBot.bot.chat_bot.data.saved;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared;
using TwitchAPI.client;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot;

public class ChatBotOptions : Options {
    private static readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private static string Name => "chat_bot";
    private static string OptionsPath => Path.Combine($"{Directories.DataDirectory}/{Name}", $"{Name}_opt.json");

    public ITwitchClient? Client { get; private set; }
    
    public override State ServiceState => State.Enabled;

    public ConnectionCredentials Credentials => _saveData!.Credentials;

    public AuthLevel AuthLevel => _saveData!.AuthLevel;
    public FullCredentials? AuthorizedCredentials => _saveData!.AuthorizedCredentials;
    

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

    public void SetAuthLevel(AuthLevel value) {
        _saveData!.AuthLevel = value;
        Save();
    }
    
    public void SetAuthorizedCredentials(FullCredentials? value) {
        _saveData!.AuthorizedCredentials = value;
        Save();
    }
}