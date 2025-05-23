using ChatBot.Services.chat_commands;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.twitchAPI.interfaces;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBot.twitchAPI;

public class ChatBot : Bot {
    private readonly ILogger<TwitchClient> _logger;
    private readonly ChatBotOptions _options = new();
    private ITwitchClient _client;
    private bool _initialized;
    private ErrorCode LogInIssues => IsValidSave();


    public override string Name { get; }
    public override ChatBotOptions Options => _options;
    public override event EventHandler<OnChatCommandReceivedArgs>? OnChatCommandReceived;
    public override event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public override event EventHandler<OnJoinedChannelArgs>? OnJoinedChannel;
    public override event EventHandler<OnConnectedArgs>? OnConnected;
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
            _client.AddChatCommandIdentifier(((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).Options.
                                             CommandIdentifier);
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

        _client.OnChatCommandReceived += OnChatCommandReceived;
        _client.OnMessageReceived += OnMessageReceived;
        _client.OnJoinedChannel += OnJoinedChannel;
        _client.OnConnected += OnConnected;
        _client.OnLog += OnLog;

        _client.Connect();
    }

    public override void Enable() {
        _options.SetState(State.Enabled);
    }

    public override void Disable() {
        _options.SetState(State.Disabled);
    }

    public override ErrorCode TryGetClient(out ITwitchClient client) {
        client = _client;
        return !_initialized ? ErrorCode.NotInitialized : ErrorCode.None;
    }

    public override ITwitchClient GetClient() {
        return _client;
    }


    public override void ToggleService() {
        _options.SetState(Options.State == State.Enabled ? State.Disabled : State.Enabled);
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