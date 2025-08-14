using ChatBot.bot.utils.GD.AREDL.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Responses;

public class LeaderboardResponse {
    [JsonProperty(PropertyName = "data")]
    public List<UserProfile?>? data;
}