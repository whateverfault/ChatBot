using ChatBot.api.client.commands;
using ChatBot.api.client.commands.data;
using ChatBot.api.client.credentials;
using ChatBot.api.event_sub;
using ChatBot.api.event_sub.subscription_data.message;
using ChatBot.api.shared.requests;
using ChatMessage = ChatBot.api.client.data.ChatMessage;

namespace ChatBot.api.client;

public enum LogLevel {
    Info,
    Error,
    Warning,
}

public class TwitchClient : ITwitchClient {
    private readonly TwitchEventSubWebSocket _websocket;
    private readonly CommandParser _commandParser;
    
    public FullCredentials? Credentials { get; private set; }

    public event EventHandler<ChatMessage>? OnMessageReceived;
    public event EventHandler<Command>? OnCommandReceived;
    public event EventHandler? OnConnected;
    public event EventHandler<string>? OnDisconnected;
    public event EventHandler<string>? OnError;
    
    
    public TwitchClient(
        TwitchEventSubWebSocket websocket,
        char? commandIdentifier = null) {
        _commandParser = new CommandParser(commandIdentifier);
        _websocket = websocket;
        Subscribe();
    }

    public async Task Initialize(FullCredentials credentials) {
        await Initialize(new ConnectionCredentials(credentials.Username, credentials.Channel, credentials.OAuth));
    }
    
    public async Task Initialize(ConnectionCredentials credentials) {
        try {
            var validateResponse = await Requests.ValidateOauth(credentials.OAuth, OnError);

            if (validateResponse == null) {
                return;
            }

            var channelInfo = await Requests.GetUserInfo(
                                                                 credentials.Channel,
                                                                 credentials.OAuth,
                                                                 validateResponse.ClientId,
                                                                 OnError);
            if (channelInfo == null) return;
            
            Credentials = new FullCredentials(
                                              credentials.Username,
                                              credentials.Channel,
                                              credentials.OAuth,
                                              validateResponse.ClientId,
                                              validateResponse.UserId,
                                              channelInfo.Id
                                             );
    
            await _websocket.ConnectAsync();
            _websocket.OnSessionStarted += SubscribeToChat;
            return;
        
            async void SubscribeToChat(object? sender, EventArgs e) {
                var result = await Requests.SubscribeToChannelChat(_websocket.SessionId, Credentials, OnError);
                if (result?.Id == null) return;
                
                _websocket.SetSubscriptionId(result.Id);
                _websocket.OnSessionStarted -= SubscribeToChat;
                OnConnected?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception e) {
            OnError?.Invoke(this, $"Error while initializing. {e.Message}");
        }
    }

    public async Task Reconnect() {
        if (Credentials == null) return;
        
        UnSubscribe();
        await Disconnect();
        await Initialize(Credentials);
    }

    public async Task Disconnect() {
        UnSubscribe();

        if (_websocket.SubscriptionId == null
            || Credentials == null) {
            return;
        }
        
        await Requests.UnsubscribeFromChannelChat(
                                                  _websocket.SubscriptionId, 
                                                  Credentials,
                                                  OnError
                                                  );
        await _websocket.DisconnectAsync();
        OnDisconnected?.Invoke(this, "Disconnected.");
    }
    
    public async Task SendMessage(string message) {
        if (Credentials == null) {
            OnError?.Invoke(this, "Couldn't send a message. Not initialized.");
            return;
        }
        
        await Requests.SendMessage(
                                   message, 
                                   Credentials, 
                                   OnError
                                   );
    }
    
    public async Task SendReply(string replyId, string message) {
        if (Credentials == null) {
            OnError?.Invoke(this, "Couldn't send a message. Not initialized.");
            return;
        }
        
        await Requests.SendReply(
                                 message,
                                 replyId,
                                 Credentials,
                                 OnError
                                 );
    }

    public bool SetCommandIdentifier(char identifier) {
        return _commandParser.SetCommandIdentifier(identifier);
    }

    private void HandleChatMessage(object? sender, ChatMessageEvent? chatMessageEvent) {
        if (chatMessageEvent == null) return;
                                                
        OnMessageReceived?.Invoke(
                                  this,
                                  ChatMessage.Create(chatMessageEvent)
                                 );
    }

    private void HandleChatCommand(object? sender, ChatMessage chatMessage) {
        var command = _commandParser.Parse(chatMessage);
        if (command == null) return;
        
        OnCommandReceived?.Invoke(
                                  this,
                                  command
                                  );
    }
    
    private void Subscribe() {
        _websocket.OnError += OnError;
        _websocket.OnChatMessageReceived += HandleChatMessage;
        _websocket.OnConnectionClosed += OnDisconnected;
        
        OnMessageReceived += HandleChatCommand;
    }

    private void UnSubscribe() {
        _websocket.OnError -= OnError;
        _websocket.OnChatMessageReceived -= HandleChatMessage;
        _websocket.OnConnectionClosed -= OnDisconnected;
        
        OnMessageReceived -= HandleChatCommand;
    }
}