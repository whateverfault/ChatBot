using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub.subscription_data.session.reconnect;

public class SessionReconnectPayload {
    [JsonProperty("session")]
    public ReconnectSession Session { get; set; }


    public SessionReconnectPayload(ReconnectSession? session = null) {
        Session = session ?? new ReconnectSession();
    }
}