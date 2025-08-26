using ChatBot.api.twitch.client;
using ChatBot.api.twitch.client.credentials;
using ChatBot.api.twitch.client.data;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;

namespace ChatBot.bot;

public delegate void NoArgs();

public class TwitchChatBot : Bot {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static TwitchChatBot? _instance;
    
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();
    
    private readonly object _startLock = new object();
    private bool _starting;
    private bool _initialized;
    private bool _canReconnect = true;


    public override string Name => "Bot";
    public override ChatBotOptions Options { get; } = new ChatBotOptions();
    
    public override event EventHandler<ChatMessage>? OnMessageReceived;
    public event NoArgs? OnInitialized;
    
    
    private TwitchChatBot(){}
    
    public override async Task StartAsync() {
        try {
            lock (_startLock) {
                if (_starting) return;
                _starting = true;
            }
            
            await Task.Run(async () => {
                               _logger.Log(LogLevel.Info, $"Connecting to {Options.Credentials.Channel}...");
                               await InitConnectionAsync();

                               if (Options.Client?.Credentials == null) {
                                   lock (_startLock) {
                                       _starting = false;
                                   }
                                   ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
                                   return;
                               }

                               SubscribeToEvents();
                               ServiceManager.InitServices();
                               lock (_startLock) {
                                   _starting = false;
                               }
                           }
                          );
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while connecting to {Options.Credentials.Channel}. {e}");
        }
    }

    public override void Start() {
        lock (_startLock) {
            _starting = true;
        }

        _logger.Log(LogLevel.Info, $"Connecting to {Options.Credentials.Channel}...");
        InitConnection();

        if (Options.Client?.Credentials == null) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
            return;
        }

        SubscribeToEvents();
        ServiceManager.InitServices();
        lock (_startLock) {
            _starting = false;
        }
    }
    
    public override void Stop() {
        if (Options.Client == null || !_initialized) {
            return;
        }
        _canReconnect = false;

        UnsubscribeFromEvents();
        Options.Client.Disconnect();

        _canReconnect = true;
        _initialized = false;
        
        _logger.Log(LogLevel.Info, "Disconnected.");
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
            
            await Options.Client.UpdateChannel(username);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while updating a channel: {e}");
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
            
            await Options.Client.UpdateOauth(oauth);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while updating an Oauth token: {e}");
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
            
            await Options.Client.UpdateChannelOauth(channelOauth);
            Options.UpdateCredentials();
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while updating a channel oauth token: {e}");
        }
    }
    
    private async Task InitConnectionAsync() {
        try {
            if (!ValidateSave()) {
                ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
                return;
            }
            
            Stop();
            
            Options.UpdateClient(new TwitchClient());
            if (Options.Client == null) return;
            
            await Options.Client.Initialize(Options.Credentials);

            var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
            Options.Client?.SetCommandIdentifier(chatCommands.Options.CommandIdentifier);
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
            
            Stop();
            
            Options.UpdateClient(new TwitchClient());
            if (Options.Client == null) return;
            
            _ = Options.Client.Initialize(Options.Credentials);

            var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
            Options.Client?.SetCommandIdentifier(chatCommands.Options.CommandIdentifier);
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    private void OnConnected(object? sender, EventArgs e) {
        _logger.Log(LogLevel.Info, $"Connected to {Options.Credentials.Channel}");
        OnInitialized?.Invoke();
    }
    
    private void Reconnect(object? sender, string message) {
        if (!_canReconnect) return;
        
        _logger.Log(LogLevel.Warning, $"{message}. Attempting reconnect...");
        Task.Delay(5000).ContinueWith(_ => Options.Client?.Reconnect());
    }
    
    private void SubscribeToEvents() {
        if (Options.Client == null) return;
        
        UnsubscribeFromEvents();
    
        Options.Client.OnMessageReceived += OnMessageReceiveHandler;
        Options.Client.OnError += OnError;
        Options.Client.OnDisconnected += Reconnect;
        Options.Client.OnConnected += OnConnected;
    }

    private void UnsubscribeFromEvents() {
        if (Options.Client == null) return;
    
        Options.Client.OnMessageReceived -= OnMessageReceiveHandler;
        Options.Client.OnDisconnected -= Reconnect;
        Options.Client.OnConnected -= OnConnected;
    }

    private void OnMessageReceiveHandler(object? sender, ChatMessage chatMessage) {
        OnMessageReceived?.Invoke(sender, chatMessage);
    }
    
    private void OnError(object? sender, string message) {
        _logger.Log(LogLevel.Error, message);
    }
    
    private bool ValidateSave() {
        return !string.IsNullOrEmpty(Options.Credentials.Channel)
            && !string.IsNullOrEmpty(Options.Credentials.Oauth);
    }
}