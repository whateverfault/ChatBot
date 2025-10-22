using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Google;

public class VertexAiMessage {
    [JsonProperty("role")]
    public readonly string Role;
    
    [JsonProperty("parts")]
    public readonly List<string> Parts;


    public VertexAiMessage(string role, List<string> parts) {
        Role = role;
        Parts = parts;
    }
}