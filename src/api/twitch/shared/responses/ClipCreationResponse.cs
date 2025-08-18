using ChatBot.api.twitch.shared.requests.data;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.responses;

public class ClipCreationResponse {
    [JsonProperty("data")]
    public List<ClipData>? Data { get; set; }
}