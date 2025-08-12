using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.session;

public class SessionWelcomePayload {
    [JsonProperty("session")]
    public SessionData Session { get; set; }
}