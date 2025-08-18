using ChatBot.api.twitch.helix.data.requests.chat_subscription;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub.subscription_data.subscription;

public class EventSubData {
    [JsonProperty("data")]
    public EventSubPayload[] Data { get; set; } 
    
    
    [JsonConstructor]
    public EventSubData(
        [JsonProperty("data")] EventSubPayload[] data) {
        Data = data;
    }
}