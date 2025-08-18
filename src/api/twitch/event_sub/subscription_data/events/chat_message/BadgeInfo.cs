using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub.subscription_data.events.chat_message;

public class BadgeInfo {
    [JsonProperty("set_id")]
    public string Name { get; set; }
    
    
    [JsonConstructor]
    public BadgeInfo(
        [JsonProperty("set_id")] string name) {
        Name = name;
    }
}