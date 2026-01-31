using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.api;
using TwitchAPI.client;
using TwitchAPI.client.commands.data;
using TwitchAPI.client.credentials;
using TwitchAPI.client.data;

namespace ChatBot.bot.chat_bot;

public delegate void NoArgs();

public sealed class TwitchChatBot : Bot {
    private static TwitchChatBot? _instance;
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();
    
    private readonly object _sync = new object();
    private bool _starting;
    private bool _initialized;

    public override TwitchApi Api { get; } = new TwitchApi(Network.HttpClient);

    public override ChatBotOptions Options { get; }

    public bool Online { get; private set; }
    
    public override event EventHandler<RewardRedemption>? OnRewardRedeemed;
    public override event EventHandler<ChatMessage>? OnMessageReceived;
    public override event EventHandler<Command>? OnCommandReceived;
    public event NoArgs? OnInitialized;

    
    private TwitchChatBot() {
        Options = new ChatBotOptions();

        var chatCommands =
            (ChatCommandsService)Services.Get(ServiceId.ChatCommands);

        var twitchClientConfig = new TwitchClientConfig(
                                                        new AutoReconnectConfig(),
                                                        Api,
                                                        chatCommands.Options.CommandIdentifier
                                                       );

        var client = new TwitchClient(twitchClientConfig);
        SetClient(client);
    }

    public async Task GetAuthorization() {
        var auth = await Authorization.GetAuthorization();

        if (!string.IsNullOrEmpty(auth.Broadcaster)) {
            await SetChannelOauth(auth.Broadcaster);
        }
        
        if (!string.IsNullOrEmpty(auth.Bot)) {
            await SetOauth(auth.Bot);
        }

        await UpdateAuthorizedCredentials();
    }

    public async Task Initialize() {
        await UpdateAuthorizedCredentials();
    }
    
    public override async Task Start() {
        lock (_sync) {
            if (_starting) {
                return;
            }

            _starting = true;
        }

        try {
            ErrorHandler.LogMessage(
                LogLevel.Info,
                $"Connecting to {Options.Credentials.Channel}..."
            );

            Services.Init();

            await InitConnectionAsync();
        }
        finally {
            lock (_sync) {
                _starting = false;
                Online = true;
            }
        }
    }

    public override async Task Stop() {
        await StopInternal();
    }

    public override ErrorCode TryGetClient(out ITwitchClient? client) {
        client = GetClient(); 
        
        return _initialized ? ErrorCode.None : ErrorCode.NotInitialized;
    }

    public override ITwitchClient? GetClient() {
        return Options.Client;
    }

    private void SetClient(TwitchClient newClient) {
        if (Options.Client != null) {
            UnsubscribeFromEvents();
        }

        Options.UpdateClient(newClient);
        SubscribeToEvents();
    }
    
    private async Task StopInternal() {
        lock (_sync) {
            Online = false;
        }
        
        if (Options.Client == null || !_initialized) {
            return;
        }
        
        Services.Kill();

        UnsubscribeFromEvents();
        await Options.Client.Disconnect();

        _initialized = false;

        ErrorHandler.LogMessage(LogLevel.Info, "Disconnected.");
    }

    private async Task InitConnectionAsync() {
        if (!ValidateSave()) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
            return;
        }

        await StopInternal();
        SubscribeToEvents();

        if (Options.Client == null) {
            return;
        }

        await Options.Client.Initialize(Options.Credentials);

        if (Options.Client.Credentials == null) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
            return;
        }

        _initialized = true;
    }

    private void SubscribeToEvents() {
        if (Options.Client == null) {
            return;
        }

        UnsubscribeFromEvents();
        
        Options.Client.OnRewardRedeemed += HandleRewardRedeemed;
        Options.Client.OnMessageReceived += HandleMessageReceived;
        Options.Client.OnCommandReceived += HandleCommandReceived;
        
        Options.Client.OnError += HandleError;
        Options.Client.OnConnected += HandleConnected;
    }

    private void UnsubscribeFromEvents() {
        if (Options.Client == null) {
            return;
        }

        Options.Client.OnRewardRedeemed -= HandleRewardRedeemed;
        Options.Client.OnMessageReceived -= HandleMessageReceived;
        Options.Client.OnCommandReceived -= HandleCommandReceived; 
        
        Options.Client.OnError -= HandleError;
        Options.Client.OnConnected -= HandleConnected;
    }

    private void HandleRewardRedeemed(object? sender, RewardRedemption message) {
        var handler = OnRewardRedeemed;
        if (handler != null) {
            handler(sender, message);
        }
    }
    
    private void HandleMessageReceived(object? sender, ChatMessage message) {
        var handler = OnMessageReceived;
        if (handler != null) {
            handler(sender, message);
        }
    }

    private void HandleCommandReceived(object? sender, Command cmd) {
        var handler = OnCommandReceived;
        if (handler != null) {
            handler(sender, cmd);
        }
    }
    
    private void HandleConnected(object? sender, EventArgs e) {
        ErrorHandler.LogMessage(
            LogLevel.Info,
            $"Connected to {Options.Credentials.Channel}"
        );

        var handler = OnInitialized;
        if (handler != null) {
            handler();
        }
    }

    private void HandleError(object? sender, string message) {
        ErrorHandler.LogMessage(LogLevel.Error, message);
    }

    public string GetChannel() {
        if (Options.Client?.Credentials?.Broadcaster.DisplayName != null) {
            return Options.Client.Credentials.Broadcaster.DisplayName;
        }

        return string.IsNullOrEmpty(Options.Credentials.Channel)?
                   "Empty" : 
                   Options.Credentials.Channel;
    }

    public async Task SetChannel(string username) {
        if (string.IsNullOrEmpty(username)) {
            return;
        }
        
        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    username,
                    Options.Credentials.Oauth,
                    Options.Credentials.BroadcasterOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateBroadcaster(username);
        Options.UpdateCredentials();
    }

    private async Task SetOauth(string oauth) {
        if (string.IsNullOrEmpty(oauth)) {
            return;
        }
        
        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    Options.Credentials.Channel,
                    oauth,
                    Options.Credentials.BroadcasterOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateOauth(oauth);
        Options.UpdateCredentials();
    }

    private async Task SetChannelOauth(string channelOauth) {
        if (string.IsNullOrEmpty(channelOauth)) {
            return;
        }

        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    Options.Credentials.Channel,
                    Options.Credentials.Oauth,
                    channelOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateBroadcasterOauth(channelOauth);
        Options.UpdateCredentials();
    }

    public AuthLevel GetAuthLevel() {
        return Options.AuthLevel;
    }
    
    public int GetAuthLevelAsInt() {
        return (int)GetAuthLevel();
    }
    
    public string GetAuthorizedBroadcasterDisplayName() {
        if (Options.AuthorizedCredentials == null
         || string.IsNullOrEmpty(Options.AuthorizedCredentials.Broadcaster.DisplayName)) {
            return "Empty";
        }
        
        return Options.AuthorizedCredentials.Broadcaster.DisplayName;
    }

    private async Task UpdateAuthorizedCredentials() {
        var client = GetClient();
        if (client == null) {
            return;
        }
        
        var authorizedCredentials = await client.GetFullCredentials(Options.Credentials);
        if (authorizedCredentials == null)
            return;
        
        var authLevel = Authorization.GetAuthorizationLevel(Options.Credentials, authorizedCredentials);
        
        Options.SetAuthorizedCredentials(authorizedCredentials);
        Options.SetAuthLevel(authLevel);
    }
    
    private bool ValidateSave() {
        return !(string.IsNullOrEmpty(Options.Credentials.Channel)
              || string.IsNullOrEmpty(Options.Credentials.Oauth));
    }
}
