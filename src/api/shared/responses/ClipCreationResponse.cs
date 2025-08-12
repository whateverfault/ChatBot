using ChatBot.api.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.shared.responses;

public class ClipCreationResponse {
    [JsonProperty("data")]
    public List<ClipData>? Data { get; set; }
}