using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.credentials;
using TwitchAPI.client.data;

namespace ChatBot.bot.chat_bot;

public delegate void NoArgs();

public sealed class TwitchChatBot : Bot {
    private static TwitchChatBot? _instance;
    
    private readonly TwitchClientConfig _twitchClientConfig;
    
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();
    
    private readonly object _startLock = new object();
    private bool _starting;
    private bool _initialized;
    
    public override ChatBotOptions Options { get; }
    
    public override event EventHandler<ChatMessage>? OnMessageReceived;
    public event NoArgs? OnInitialized;


    private TwitchChatBot() {
        Options = new ChatBotOptions();
        
        Options.UpdateClient(new TwitchClient(_twitchClientConfig));
        
        var chatCommands = (ChatCommandsService)Services.Get(ServiceId.ChatCommands);
        _twitchClientConfig = new TwitchClientConfig(
                                                     new AutoReconnectConfig(),
                                                     chatCommands.Options.CommandIdentifier
                                                     );
    }
    
    public override async Task StartAsync() {
        try {
            lock (_startLock) {
                if (_starting) return;
                _starting = true;
            }
            
            await Task.Run(async () => {
                               ErrorHandler.LogMessage(LogLevel.Info, $"Connecting to {Options.Credentials.Channel}...");
                               
                               Services.Init();
                               
                               SubscribeToEvents();
                               await InitConnectionAsync();

                               if (Options.Client?.Credentials == null) {
                                   lock (_startLock) {
                                       _starting = false;
                                   }
                                   ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
                                   return;
                               }
                               
                               lock (_startLock) {
                                   _starting = false;
                               }
                           }
                           );
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while connecting to {Options.Credentials.Channel}. {e.Data}");
        }
    }

    public override void Start() {
        lock (_startLock) {
            if (_starting) {
                return;
            }

            _starting = true;
        }

        ErrorHandler.LogMessage(LogLevel.Info, $"Connecting to {Options.Credentials.Channel}...");

        Services.Init();
        
        SubscribeToEvents();
        InitConnection();

        if (Options.Client?.Credentials == null) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
            return;
        }
        
        lock (_startLock) {
            _starting = false;
        }
        
        
    }

    public override void Stop() {
        StopInternal();
    }
    
    private bool StopInternal() {
        if (Options.Client == null || !_initialized) {
            return false;
        }

        Services.Kill();
        
        UnsubscribeFromEvents();
        Options.Client.Disconnect();
        
        _initialized = false;
        
        ErrorHandler.LogMessage(LogLevel.Info, "Disconnected.");
        return true;
    }

    public override ErrorCode TryGetClient(out ITwitchClient? client) {
        client = GetClient();
        return !_initialized ? ErrorCode.NotInitialized : ErrorCode.None;
    }

    public override ITwitchClient? GetClient() {
        return Options.Client;
    }

    public string GetChannel() {
        return Options.Client?.Credentials?.Channel ??
               (string.IsNullOrEmpty(Options.Credentials.Channel)? 
                    "Empty": 
                    Options.Credentials.Channel);
    }

    public async void SetChannel(string username) {
        try {
            if (Options.Client?.Credentials == null) {
                Options.SetCredentials(
                                       new ConnectionCredentials
                                           (
                                            username,
                                            Options.Credentials.Oauth,
                                            Options.Credentials.ChannelOauth
                                            )
                                       );
                return;
            }
            
            if (string.IsNullOrEmpty(username)) {
                ErrorHandler.PrintMessage("Stop the bot first.");
                return;
            }
            
            await Options.Client.UpdateChannel(username);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while updating a channel: {e.Data}");
        }
    }
    
    public string GetOauth() {
        return Options.Client?.Credentials?.Oauth ??
               (string.IsNullOrEmpty(Options.Credentials.Oauth)? 
                    "Empty": 
                    Options.Credentials.Oauth);
    }

    public async void SetOauth(string oauth) {
        try {
            if (Options.Client?.Credentials == null) {
                Options.SetCredentials(
                                       new ConnectionCredentials
                                           (
                                            Options.Credentials.Channel,
                                            oauth,
                                            Options.Credentials.ChannelOauth
                                           )
                                      );
                return;
            }

            if (string.IsNullOrEmpty(oauth)) {
                ErrorHandler.PrintMessage("Stop the bot first.");
                return;
            }
            
            await Options.Client.UpdateOauth(oauth);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while updating an Oauth token: {e.Data}");
        }
    }
    
    public string GetChannelOauth() {
        return Options.Client?.Credentials?.ChannelOauth ??
               (string.IsNullOrEmpty(Options.Credentials.ChannelOauth)? 
                    "Empty": 
                    Options.Credentials.ChannelOauth);
    }

    public async void SetChannelOauth(string channelOauth) {
        try {
            if (Options.Client?.Credentials == null) {
                Options.SetCredentials(
                                       new ConnectionCredentials
                                           (
                                            Options.Credentials.Channel,
                                            Options.Credentials.Oauth,
                                            channelOauth
                                           )
                                      );
                return;
            }
            
            if (string.IsNullOrEmpty(channelOauth)) {
                ErrorHandler.PrintMessage("Stop the bot first.");
                return;
            }
            
            await Options.Client.UpdateChannelOauth(channelOauth);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while updating a channel oauth token: {e.Data}");
        }
    }
    
    private async Task InitConnectionAsync() {
        try {
            if (!ValidateSave()) {
                ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
                return;
            }

            if (StopInternal()) {
                SubscribeToEvents();
            }
            
            if (Options.Client == null) return;
            
            await Options.Client.Initialize(Options.Credentials);
            
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    private void InitConnection() {
        try {
            if (!ValidateSave()) {
                ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
                return;
            }
            
            if (StopInternal()) {
                SubscribeToEvents();
            }
            
            if (Options.Client == null) return;
            
            _ = Options.Client.Initialize(Options.Credentials);
            
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    private void OnConnected(object? sender, EventArgs e) {
        ErrorHandler.LogMessage(LogLevel.Info, $"Connected to {Options.Credentials.Channel}");
        OnInitialized?.Invoke();
    }
    
    private void SubscribeToEvents() {
        if (Options.Client == null) return;
        
        UnsubscribeFromEvents();
    
        Options.Client.OnMessageReceived += OnMessageReceiveHandler;
        Options.Client.OnError += OnError;
        Options.Client.OnConnected += OnConnected;
    }

    private void UnsubscribeFromEvents() {
        if (Options.Client == null) return;
    
        Options.Client.OnMessageReceived -= OnMessageReceiveHandler;
        Options.Client.OnError -= OnError;
        Options.Client.OnConnected -= OnConnected;
    }

    private void OnMessageReceiveHandler(object? sender, ChatMessage chatMessage) {
        OnMessageReceived?.Invoke(sender, chatMessage);
    }
    
    private void OnError(object? sender, string message) {
        ErrorHandler.LogMessage(LogLevel.Error, message);
    }
    
    private bool ValidateSave() {
        return !string.IsNullOrEmpty(Options.Credentials.Channel)
            && !string.IsNullOrEmpty(Options.Credentials.Oauth);
    }
}