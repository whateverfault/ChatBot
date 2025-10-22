using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.HuggingFace.data;

public class Message {
    [JsonProperty("role")]
    public string? Role { get; set; }
    
    [JsonProperty("content")]
    public string? Content { get; set; }
}