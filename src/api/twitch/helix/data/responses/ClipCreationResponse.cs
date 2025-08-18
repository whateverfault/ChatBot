using ChatBot.api.twitch.helix.data.requests;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.responses;

public class ClipCreationResponse {
    [JsonProperty("data")]
    public List<ClipData>? Data { get; set; }
}