using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data.ChatSubscriptionRequest;

public class EventSubPayload {
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string? Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; private set; }
    
    [JsonProperty("version")]
    public string Version { get; private set; }
    
    [JsonProperty("condition")]
    public Condition Condition { get; private set; }
    
    [JsonProperty("transport")]
    public Transport Transport { get; private set; }


    [JsonConstructor]
    public EventSubPayload(
        [JsonProperty("type")] string type,
        [JsonProperty("version")] string version,
        [JsonProperty("condition")] Condition condition,
        [JsonProperty("transport")] Transport transport,
        [JsonProperty("id")] string? id = null) {
        Id = id;
        Type = type;
        Version = version;
        Condition = condition;
        Transport = transport;
    }
}