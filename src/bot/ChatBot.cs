using ChatBot.bot.interfaces;
using ChatBot.Services.chat_commands;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using LogLevel = ChatBot.Services.logger.LogLevel;

namespace ChatBot.bot;

public delegate void NoArgs();

public class ChatBot : Bot {
    private static readonly LoggerService _messageLogger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private readonly ILogger<TwitchClient> _logger = null!;
    private readonly ChatBotOptions _options = new();
    private ITwitchClient? _client;
    private bool _initialized;
    private ErrorCode LogInIssues => IsValidSave();


    public override string Name => "Bot";
    public override ChatBotOptions Options => _options;
    public override event EventHandler<OnChatCommandReceivedArgs>? OnChatCommandReceived;
    public override event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public override event EventHandler<OnLogArgs>? OnLog;
    
    
    private void Init() {
        try {
            var credentials = new ConnectionCredentials(_options.Username, _options.OAuth);
            var clientOptions = new ClientOptions {
                                                      MessagesAllowedInPeriod = 750,
                                                      ThrottlingPeriod = TimeSpan.FromSeconds(30)
                                                  };
            var customClient = new WebSocketClient(clientOptions);

            _client = new TwitchClient(customClient, logger: _logger);
            _client.Initialize(credentials, _options.Channel);
            _client.RemoveChatCommandIdentifier('!');
            _client.AddChatCommandIdentifier(((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).Options.CommandIdentifier);
            _initialized = true;
        } catch (Exception) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
        }
    }
    
    public override void Init(Bot bot){}
    
    public override void Start() {
        if (ErrorHandler.LogErrorAndPrint(LogInIssues)) {
            return;
        }

        Init();
        
        _client!.OnChatCommandReceived += OnChatCommandReceived;
        _client.OnMessageReceived += OnMessageReceived;
        _client.OnLog += OnLog;
        
        _client.Connect();
        _messageLogger.Log(LogLevel.Info, $"Connected to {Options.Channel}");
        _messageLogger.Log(LogLevel.Info, $"Bot Username: {Options.Username}");
    }
    
    public override void Enable() {
        _options.SetState(State.Enabled);
    }

    public override void Disable() {
        _options.SetState(State.Disabled);
    }
    
    public override ErrorCode TryGetClient(out ITwitchClient? client) {
        client = GetClient();
        return !_initialized ? ErrorCode.NotInitialized : ErrorCode.None;
    }

    public override ITwitchClient? GetClient() {
        return _client;
    }

    public override void ToggleService() {
        _options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }
    
    private ErrorCode IsValidSave() {
        if (string.IsNullOrEmpty(_options.Username)
            || string.IsNullOrEmpty(_options.OAuth)
            || string.IsNullOrEmpty(_options.Channel)) {
            return ErrorCode.LogInIssue;
        }

        return ErrorCode.None;
    }
}