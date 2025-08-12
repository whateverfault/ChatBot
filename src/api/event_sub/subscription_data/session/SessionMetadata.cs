using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.session;

public class SessionMetadata {
    [JsonProperty("message_id")]
    public string MessageId { get; set; }

    [JsonProperty("message_type")]
    public string MessageType { get; set; }

    [JsonProperty("subscription_type")]
    public string SubscriptionType { get; set; }
}