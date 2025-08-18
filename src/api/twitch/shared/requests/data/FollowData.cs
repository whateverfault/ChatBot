using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data;

public class FollowData {
    [JsonProperty("followed_at")]
    public DateTime FollowedAt { get; set; }
}