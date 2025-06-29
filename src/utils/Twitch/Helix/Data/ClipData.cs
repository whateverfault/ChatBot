using Newtonsoft.Json;

namespace ChatBot.utils.Twitch.Helix.Data;

public class ClipData {
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("edit_url")]
    public string? EditUrl { get; set; }
}