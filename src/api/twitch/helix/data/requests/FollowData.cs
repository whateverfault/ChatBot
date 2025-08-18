using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.requests;

public class FollowData {
    [JsonProperty("followed_at")]
    public DateTime FollowedAt { get; set; }
}