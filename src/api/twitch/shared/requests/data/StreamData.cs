using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data;

public class StreamData {
    [JsonProperty("title")]
    public string Title { get; set; } = null!;
}