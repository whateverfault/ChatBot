using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class BadgeInfo {
    [JsonProperty("set_id")]
    public string Name { get; set; }
    
    
    [JsonConstructor]
    public BadgeInfo(
        [JsonProperty("set_id")] string name) {
        Name = name;
    }
}