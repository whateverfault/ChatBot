using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.Google.Data;

public class Candidate {
    [JsonProperty("content")]
    public Content? Content { get; set; }
}