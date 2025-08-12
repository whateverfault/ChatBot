using ChatBot.api.client;
using ChatBot.api.client.credentials;
using ChatBot.api.client.data;
using ChatBot.api.event_sub;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.bot;

public delegate void NoArgs();

public class TwitchChatBot : Bot {
    private static readonly LoggerService _messageLogger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static TwitchChatBot? _instance;
    
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();
    
    private ITwitchClient? _client;
    
    private readonly object _startLock = new object();
    private bool _starting;
    private bool _initialized;
    private bool _canReconnect = true;


    public override string Name => "Bot";
    public override ChatBotOptions Options { get; } = new ChatBotOptions();
    
    public override event EventHandler<ChatMessage>? OnMessageReceived;
    public event NoArgs? OnInitialized;
    
    
    private TwitchChatBot(){}
    
    private void InitConnection() {
        try {
            if (!ValidateSave()) {
                ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
                return;
            }

            var credentials = new ConnectionCredentials(Options.Username!, Options.Channel!, Options.OAuth!);
            var websocket = new TwitchEventSubWebSocket();

            Stop();
            
            _client = new TwitchClient(websocket);
            _client.Initialize(credentials);

            var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
            _client.SetCommandIdentifier(chatCommands.Options.CommandIdentifier);
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    public override async void Start() {
        try {
            lock (_startLock) {
                if (_starting) return;
                _starting = true;
            }
            
            await Task.Run(() => {
                               _messageLogger.Log(LogLevel.Info, $"Connecting to {Options.Channel}...");
                               InitConnection();

                               if (_client == null) {
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
            _messageLogger.Log(LogLevel.Error, $"Exception while connecting to {Options.Channel}. {e}");
        }
    }

    public override void Stop() {
        if (_client == null || !_initialized) {
            return;
        }
        _canReconnect = false;

        UnsubscribeFromEvents();
        _client.Disconnect();

        _canReconnect = true;
        _initialized = false;
        
        _messageLogger.Log(LogLevel.Info, "Disconnected.");
    }
    
    public override void Enable() {
        Options.SetState(State.Enabled);
    }

    public override void Disable() {
        Options.SetState(State.Disabled);
    }

    public override ErrorCode TryGetClient(out ITwitchClient? client) {
        client = GetClient();
        return !_initialized ? ErrorCode.NotInitialized : ErrorCode.None;
    }

    public override ITwitchClient? GetClient() {
        return _client;
    }

    private void OnConnected(object? sender, EventArgs e) {
        _messageLogger.Log(LogLevel.Info, $"Connected to {Options.Channel}");
        _messageLogger.Log(LogLevel.Info, $"Bot Username: {Options.Username}");
        
        OnInitialized?.Invoke();
    }
    
    private void Reconnect(object? sender, string message) {
        if (!_canReconnect) return;
        
        _messageLogger.Log(LogLevel.Warning, $"{message} Attempting reconnect...");
        Task.Delay(5000).ContinueWith(_ => _client?.Reconnect());
    }
    
    private void SubscribeToEvents() {
        UnsubscribeFromEvents();
    
        _client!.OnMessageReceived += OnMessageReceived;
        _client.OnDisconnected += Reconnect;
        _client.OnConnected += OnConnected;
    }

    private void UnsubscribeFromEvents() {
        if (_client == null) return;
    
        _client.OnMessageReceived -= OnMessageReceived;
        _client.OnDisconnected -= Reconnect;
        _client.OnConnected -= OnConnected;
    }
    
    private bool ValidateSave() {
        if (string.IsNullOrEmpty(Options.Username)
            || string.IsNullOrEmpty(Options.OAuth)
            || string.IsNullOrEmpty(Options.Channel)) {
            return false;
        }

        return true;
    }
}