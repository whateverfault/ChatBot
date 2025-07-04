using ChatBot.utils.Twitch.Helix.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.Twitch.Helix.Responses;

public class RewardCreationResponse {
    [JsonProperty("data")]
    public List<RewardData> Data { get; set; } = null!;
}
