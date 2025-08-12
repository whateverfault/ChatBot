using ChatBot.bot.services.ai.AiClients.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.Google.Response;

public class VertexAiResponse {
    [JsonProperty("candidates")]
    public List<Candidate>? Candidates { get; set; }
}