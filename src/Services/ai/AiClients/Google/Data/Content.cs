using Newtonsoft.Json;

namespace ChatBot.Services.ai.AiClients.Google.Data;

public class Content {
    [JsonProperty("parts")]
    public Part[] Parts { get; set; } = null!;
}