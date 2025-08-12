using ChatBot.api.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.shared.responses;

public class StreamResponse {
    [JsonProperty("data")]
    public List<StreamData?> Data { get; set; } = null!;
}