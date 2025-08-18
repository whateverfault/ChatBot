using ChatBot.api.twitch.event_sub.subscription_data.subscription;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub.subscription_data.events;

public class EventSubMessagePayload<TEvent> {
    [JsonProperty("subscription")]
    public SubscriptionData Subscription { get; set; }

    [JsonProperty("event")]
    public TEvent Event { get; set; }
    
    
    [JsonConstructor]
    public EventSubMessagePayload(
        [JsonProperty("subscription")] SubscriptionData subscription,
        [JsonProperty("event")] TEvent e) {
        Subscription = subscription;
        Event = e;
    }
}