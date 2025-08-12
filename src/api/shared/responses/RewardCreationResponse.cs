using ChatBot.api.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.shared.responses;

public class RewardCreationResponse {
    [JsonProperty("data")]
    public List<RewardData> Data { get; set; } = null!;
}
