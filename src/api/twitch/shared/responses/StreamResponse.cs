using ChatBot.api.twitch.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.responses;

public class StreamResponse {
    [JsonProperty("data")]
    public List<StreamData?> Data { get; set; } = null!;
}