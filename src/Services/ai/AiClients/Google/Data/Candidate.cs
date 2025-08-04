using Newtonsoft.Json;

namespace ChatBot.services.ai.AiClients.Google.Data;

public class Candidate {
    [JsonProperty("content")]
    public Content? Content { get; set; }
}