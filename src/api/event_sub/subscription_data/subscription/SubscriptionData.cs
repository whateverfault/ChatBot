using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.subscription;

public class SubscriptionData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}