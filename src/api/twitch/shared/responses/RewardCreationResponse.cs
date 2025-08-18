using ChatBot.api.twitch.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.responses;

public class RewardCreationResponse {
    [JsonProperty("data")]
    public List<RewardData> Data { get; set; } = null!;
}
