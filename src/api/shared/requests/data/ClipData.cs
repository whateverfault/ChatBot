using Newtonsoft.Json;

namespace ChatBot.api.shared.requests.data;

public class ClipData {
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("edit_url")]
    public string? EditUrl { get; set; }
}