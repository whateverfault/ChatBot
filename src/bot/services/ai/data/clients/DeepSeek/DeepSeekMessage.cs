using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.DeepSeek;

public class DeepSeekMessage {
    [JsonProperty("role")]
    public readonly string Role;
    
    [JsonProperty("content")]
    public readonly string Content;


    public DeepSeekMessage(string role, string content) {
        Role = role;
        Content = content;
    }
}