using ChatBot.api.twitch.helix.data.requests;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.responses;

public class StreamResponse {
    [JsonProperty("data")]
    public List<StreamData?> Data { get; set; } = null!;
}