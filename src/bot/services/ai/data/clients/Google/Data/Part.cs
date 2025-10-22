using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Google.Data;

public class Part {
    [JsonProperty("text")]
    public string Text { get; set; } = null!;
}