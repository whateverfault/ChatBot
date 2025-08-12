using Newtonsoft.Json;

namespace ChatBot.api.shared.requests.data.ChatSubscriptionRequest;

public class Condition {
    [JsonProperty("broadcaster_user_id")]
    public string BroadcasterId { get; private set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; private set; }
    
    
    public Condition(string broadcasterId, string userId) {
        BroadcasterId = broadcasterId;
        UserId = userId;
    }
}