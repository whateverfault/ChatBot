using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data;

public class ClipData {
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("edit_url")]
    public string? EditUrl { get; set; }
}