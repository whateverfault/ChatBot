using ChatBot.api.twitch.client.commands.data;
using ChatBot.api.twitch.client.credentials;
using ChatBot.api.twitch.client.data;

namespace ChatBot.api.twitch.client;

public interface ITwitchClient {
    public FullCredentials? Credentials { get; }

    public event EventHandler<ChatMessage>? OnMessageReceived;
    public event EventHandler<Command>? OnCommandReceived;
    public event EventHandler? OnConnected;
    public event EventHandler<string>? OnDisconnected;
    public event EventHandler<string>? OnError;

    
    public Task Initialize(ConnectionCredentials credentials);

    public Task Reconnect();
    
    public Task Disconnect();
    
    public Task SendMessage(string message);
    
    public Task SendReply(string replyId, string message);

    public bool SetCommandIdentifier(char identifier);

    public Task UpdateChannel(string username);

    public Task UpdateOauth(string oauth);

    public Task UpdateChannelOauth(string oauth);
}