using System.Net.WebSockets;
using System.Text;
using ChatBot.api.event_sub.subscription_data.message;
using ChatBot.api.event_sub.subscription_data.session;
using Newtonsoft.Json;

namespace ChatBot.api.event_sub;

public class TwitchEventSubWebSocket {
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private ClientWebSocket? _webSocket;
    
    public event EventHandler<ChatMessageEvent?>? OnChatMessageReceived;
    public event EventHandler<string>? OnConnectionClosed;
    public event EventHandler? OnSessionStarted;
    public event EventHandler<string>? OnError;
    
    public string? SessionId { get; private set; }
    public string? SubscriptionId { get; private set; }
    

    public async Task ConnectAsync() {
        _webSocket = new ClientWebSocket();
        await _webSocket.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws"), _cancellationTokenSource.Token);
        
        _ = Task.Run(ReceiveMessagesAsync, _cancellationTokenSource.Token);
    }

    private async Task ReceiveMessagesAsync() {
        var buffer = new byte[4096];
        
        while (_webSocket?.State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested) {
            try {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                
                if (result.MessageType == WebSocketMessageType.Close) {
                    OnConnectionClosed?.Invoke(this, "WebSocket closed by server");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                HandleMessage(message);
            }
            catch (Exception ex) {
                OnConnectionClosed?.Invoke(this, $"Error: {ex.Message}");
                break;
            }
        }
    }

    private void HandleMessage(string message) {
        try {
            var baseMessage =
                JsonConvert.DeserializeObject<EventSubMessage<SessionMetadata, SessionWelcomePayload>>(message);

            if (baseMessage == null) {
                OnError?.Invoke(this, "Couldn't deserialize the message.");
                return;
            }

            switch (baseMessage.Metadata.MessageType) {
                case "session_welcome":
                    var welcomeMessage =
                        JsonConvert
                           .DeserializeObject<EventSubMessage<SessionMetadata, SessionWelcomePayload>>(message);

                    if (welcomeMessage == null) {
                        OnError?.Invoke(this, "Couldn't deserialize the welcome message");
                        return;
                    }

                    SessionId = welcomeMessage.Payload.Session.Id;
                    OnSessionStarted?.Invoke(this, EventArgs.Empty);
                    break;

                case "notification" when baseMessage.Metadata.SubscriptionType == "channel.chat.message":
                    var chatMessage =
                        JsonConvert
                           .DeserializeObject<EventSubMessage<SessionMetadata, ChatMessagePayload>>(message);
                    OnChatMessageReceived?.Invoke(this, chatMessage?.Payload.Event);
                    break;

                case "session_keepalive":
                    break;

                default:
                    OnError?.Invoke(this, $"Unhandled message type: {baseMessage.Metadata.MessageType}");
                    break;
            }
        }
        catch (Exception ex) {
            OnError?.Invoke(this, $"Error handling message: {ex.Message}");
        }
    }

    public async Task DisconnectAsync() {
        if (_webSocket?.State == WebSocketState.Open) {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
        await _cancellationTokenSource.CancelAsync();
    }

    public void SetSubscriptionId(string id) {
        SubscriptionId = id;
    }
}