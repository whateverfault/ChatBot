using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.requests;

public class StreamData {
    [JsonProperty("title")]
    public string Title { get; set; } = null!;
}