using ChatBot.utils.Helix.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.Helix.Responses;

public class ClipCreationResponse {
    [JsonProperty("data")]
    public List<ClipData>? Data { get; set; }
}