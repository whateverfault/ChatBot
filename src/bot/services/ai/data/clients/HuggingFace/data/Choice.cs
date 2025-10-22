using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.HuggingFace.data;

public class Choice {
    [JsonProperty("id")]
    public int Index { get; set; }
    
    [JsonProperty("message")]
    public Message? Message { get; set; }
    
    [JsonProperty("finish_reason")]
    public string? FinishReason { get; set; }
}
