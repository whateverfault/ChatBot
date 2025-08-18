using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class LeaderboardResponse {
    [JsonProperty(PropertyName = "data")]
    public List<UserProfile?>? data;
}