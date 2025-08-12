using ChatBot.api.shared.requests.data.ChatSubscriptionRequest;
using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.subscription;

public class ChatSubscriptionData {
    [JsonProperty("data")]
    public ChatSubscriptionPayload[] Data { get; set; } 
}