using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.Google.Data;

public class Content {
    [JsonProperty("parts")]
    public Part[] Parts { get; set; } = null!;
}