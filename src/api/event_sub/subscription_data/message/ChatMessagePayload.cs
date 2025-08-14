using ChatBot.api.event_sub.subscription_data.subscription;
using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class ChatMessagePayload {
    [JsonProperty("subscription")]
    public SubscriptionData Subscription { get; set; }

    [JsonProperty("event")]
    public ChatMessageEvent Event { get; set; }
    
    
    [JsonConstructor]
    public ChatMessagePayload(
        [JsonProperty("subscription")] SubscriptionData subscription,
        [JsonProperty("event")] ChatMessageEvent e) {
        Subscription = subscription;
        Event = e;
    }
}