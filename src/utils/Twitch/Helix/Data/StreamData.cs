using Newtonsoft.Json;

namespace ChatBot.utils.Twitch.Helix.Data;

public class StreamData {
    [JsonProperty("title")]
    public string Title { get; set; } = null!;
}