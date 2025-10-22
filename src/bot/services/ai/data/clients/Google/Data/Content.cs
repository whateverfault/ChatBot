using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Google.Data;

public class Content {
    [JsonProperty("parts")]
    public Part[] Parts { get; set; } = null!;
}