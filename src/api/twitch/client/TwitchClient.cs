using ChatBot.api.twitch.client.commands;
using ChatBot.api.twitch.client.commands.data;
using ChatBot.api.twitch.client.credentials;
using ChatBot.api.twitch.event_sub;
using ChatBot.api.twitch.event_sub.subscription_data.events.chat_message;
using ChatBot.api.twitch.shared.requests;
using ChatMessage = ChatBot.api.twitch.client.data.ChatMessage;

namespace ChatBot.api.twitch.client;

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
    
    
    public TwitchClient(char? commandIdentifier = null) {
        _commandParser = new CommandParser(commandIdentifier);
        _websocket = new TwitchEventSubWebSocket();
        Subscribe();
    }

    public async Task Initialize(FullCredentials credentials) {
        await Initialize(new ConnectionCredentials(credentials.Username, credentials.Channel, credentials.Oauth));
    }
    
    public async Task Initialize(ConnectionCredentials credentials) {
        try {
            var validateResponse = await TwitchRequests.ValidateOauth(credentials.Oauth, OnError);

            if (validateResponse == null) {
                return;
            }

            var channelInfo = await TwitchRequests.GetUserInfo(
                                                         credentials.Channel,
                                                         credentials.Oauth,
                                                         validateResponse.ClientId,
                                                         OnError
                                                         );
            if (channelInfo == null) return;
            
            Credentials = new FullCredentials(
                                              validateResponse.Login,
                                              credentials.Channel,
                                              credentials.Oauth,
                                              credentials.ChannelOauth,
                                              validateResponse.ClientId,
                                              validateResponse.UserId,
                                              channelInfo.Id
                                             );
    
            await _websocket.ConnectAsync();
            _websocket.OnSessionStarted += SubscribeToChat;
            return;
        
            async void SubscribeToChat(object? sender, EventArgs e) {
                try {
                    var result = await TwitchRequests.SubscribeToChannelChat(_websocket.SessionId, Credentials, OnError);
                    if (result?.Id == null) return;
                
                    _websocket.SetSubscriptionId(result.Id);
                    _websocket.OnSessionStarted -= SubscribeToChat;
                    OnConnected?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex) {
                    OnError?.Invoke(this, $"Error while subscribing to a chat. {ex.Message}");
                }
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
        
        await TwitchRequests.EventSubUnSubscribe(
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
        
        await TwitchRequests.SendMessage(
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
        
        await TwitchRequests.SendReply(
                                 message,
                                 replyId,
                                 Credentials,
                                 OnError
                                 );
    }

    public bool SetCommandIdentifier(char identifier) {
        return _commandParser.SetCommandIdentifier(identifier);
    }

    public async Task UpdateChannel(string username) {
        var userId = await ValidateUser(username, OnError);
        if (userId == null) return;
        
        Credentials?.UpdateChannel(username);
        Credentials?.UpdateChannelId(userId);
    }
    
    public async Task UpdateOauth(string oauth) {
        var response = await TwitchRequests.ValidateOauth(oauth, OnError);
        if (response == null) return;
        
        Credentials?.UpdateOauth(oauth);
        Credentials?.UpdateUsername(response.Login);
        Credentials?.UpdateUserId(response.UserId);
        Credentials?.UpdateClientId(response.ClientId);
    }

    public async Task UpdateChannelOauth(string oauth) {
        var response = await TwitchRequests.ValidateOauth(oauth, OnError);
        if (response == null) return;
        
        Credentials?.UpdateChannelOauth(oauth);
        Credentials?.UpdateChannel(response.Login);
        Credentials?.UpdateChannelId(response.UserId);
    }
    
    private async Task<string?> ValidateUser(string username, EventHandler<string>? errorCallback = null) {
        if (Credentials == null) {
            errorCallback?.Invoke(this, "Cannot update a username before initializing a client.");
            return null;
        }
        
        var userId = await TwitchRequests.GetUserId(username, Credentials);
        if (userId == null) {
            errorCallback?.Invoke(this, "User doesn't exist.");
        }

        return userId;
    }
    
    private void HandleChatMessage(object? sender, ChatMessageEvent? chatMessageEvent) {
        if (chatMessageEvent == null
            || Credentials == null
            || chatMessageEvent.UserId.Equals(Credentials.UserId)) return;
                                                
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
        _websocket.OnWebSocketError += OnWebSocketError;
        _websocket.OnChatMessageReceived += HandleChatMessage;
        _websocket.OnConnectionClosed += OnConnectionClosed;
        
        OnMessageReceived += HandleChatCommand;
    }

    private void UnSubscribe() {
        _websocket.OnWebSocketError -= OnError;
        _websocket.OnChatMessageReceived -= HandleChatMessage;
        _websocket.OnConnectionClosed -= OnDisconnected;
        
        OnMessageReceived -= HandleChatCommand;
    }

    private void OnWebSocketError(object? sender, string message) {
        OnError?.Invoke(sender, message);
    }
    
    private void OnConnectionClosed(object? sender, string message) {
        OnDisconnected?.Invoke(sender, message);
    }
}