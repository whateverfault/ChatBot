using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data.ChatSubscriptionRequest;

public class Transport {
    [JsonProperty("method")]
    public string Method { get; private set; }
    
    [JsonProperty("session_id")]
    public string? SessionId { get; private set; }
    
    
    public Transport(string method, string? sessionId) {
        Method = method;
        SessionId = sessionId;
    }
}