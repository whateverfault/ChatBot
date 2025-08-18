using ChatBot.api.twitch.helix.data.requests;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.responses;

public class RewardCreationResponse {
    [JsonProperty("data")]
    public List<RewardData> Data { get; set; } = null!;
}
