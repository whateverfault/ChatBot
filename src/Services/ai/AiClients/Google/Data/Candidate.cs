using Newtonsoft.Json;

namespace ChatBot.Services.ai.AiClients.Google.Data;

public class Candidate {
    [JsonProperty("content")]
    public Content? Content { get; set; }
}