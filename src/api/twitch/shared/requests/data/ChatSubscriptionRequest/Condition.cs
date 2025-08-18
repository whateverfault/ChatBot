using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data.ChatSubscriptionRequest;

public class Condition {
    [JsonProperty("broadcaster_user_id")]
    public string BroadcasterId { get; private set; }
    
    [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserId { get; private set; }
    
    
    public Condition(string broadcasterId, string? userId = null) {
        BroadcasterId = broadcasterId;
        UserId = userId;
    }
}