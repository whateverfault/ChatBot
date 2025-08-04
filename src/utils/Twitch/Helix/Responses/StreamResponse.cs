using ChatBot.utils.Twitch.Helix.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.Twitch.Helix.Responses;

public class StreamResponse {
    [JsonProperty("data")]
    public List<StreamData?> Data { get; set; } = null!;
}