using Newtonsoft.Json;

namespace ChatBot.utils.Helix.Data;

public class FollowData {
    [JsonProperty("followed_at")]
    public DateTime FollowedAt { get; set; }
}