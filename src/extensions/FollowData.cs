using Newtonsoft.Json;

namespace ChatBot.extensions;

public class FollowData {
    [JsonProperty("followed_at")]
    public DateTime FollowedAt { get; set; }
}