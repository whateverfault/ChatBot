using ChatBot.utils.Twitch.Helix.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.Twitch.Helix.Responses;

public class ClipCreationResponse {
    [JsonProperty("data")]
    public List<ClipData>? Data { get; set; }
}