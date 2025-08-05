using ChatBot.bot.@interface;
using ChatBot.services.chat_commands;
using ChatBot.services.logger;
using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using LogLevel = ChatBot.services.logger.LogLevel;

namespace ChatBot.bot;

public delegate void NoArgs();

public class TwitchChatBot : Bot {
    private static TwitchChatBot? _instance;
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();
    
    private static readonly LoggerService _messageLogger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private readonly ILogger<TwitchClient> _logger = null!;
    private ITwitchClient? _client;
    private bool _starting;
    private bool _initialized;
    private bool _shouldReconnect = true;
    private long _lastTimeTriedReconnect;


    public override string Name => "Bot";
    public override ChatBotOptions Options { get; } = new ChatBotOptions();
    
    public override event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public override event EventHandler<OnLogArgs>? OnLog;
    public event NoArgs? OnInitialized;
    
    
    private TwitchChatBot(){}
    
    private void InitConnection() {
        try {
            var credentials = new ConnectionCredentials(Options.Username, Options.OAuth);
            var clientOptions = new ClientOptions {
                                                      MessagesAllowedInPeriod = 750,
                                                      ThrottlingPeriod = TimeSpan.FromSeconds(30),
                                                  };
            var customClient = new WebSocketClient(clientOptions);

            Stop();
            
            _client = new TwitchClient(customClient, logger: _logger);
            _client.Initialize(
                               credentials, 
                               Options.Channel,
                               ' ',
                               ' ');
            
            _client.AddChatCommandIdentifier(((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).Options.CommandIdentifier);
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    public override async void Start() {
        try {
            if (_starting) return;
            
            await Task.Run(() => {
                               _starting = true;
                               if (!ValidateSave()) {
                                   ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
                                   return;
                               }
                     
                               InitConnection();

                               if (_client == null) {
                                   ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
                                   return;
                               }
                         
                               _client.OnMessageReceived += OnMessageReceived;
                               _client.OnDisconnected += Reconnect;
                               _client.OnConnected += OnConnected;
                               _client.OnLog += OnLog;

                               _messageLogger.Log(LogLevel.Info, $"Connecting to {Options.Channel}...");
                               if (!_client.Connect()) {
                                   ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
                               }
                               _starting = false;
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

        _client.OnMessageReceived -= OnMessageReceived;
        _client.OnDisconnected -= Reconnect;
        _client.OnConnected -= OnConnected;
        _client.OnLog -= OnLog;
        
        _shouldReconnect = false;
        _client.Disconnect();
        _shouldReconnect = true;

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

    private void OnConnected(object? sender, OnConnectedArgs args) {
        _messageLogger.Log(LogLevel.Info, $"Connected to {Options.Channel}");
        _messageLogger.Log(LogLevel.Info, $"Bot Username: {Options.Username}");
        
        OnInitialized?.Invoke();
    }
    
    private void Reconnect(object? sender, OnDisconnectedEventArgs args) {
        if (!_shouldReconnect) return;
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - _lastTimeTriedReconnect < 60) return;
        _lastTimeTriedReconnect = now;
        
        _messageLogger.Log(LogLevel.Info, "Reconnect attempted.");
        _client?.Reconnect();
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