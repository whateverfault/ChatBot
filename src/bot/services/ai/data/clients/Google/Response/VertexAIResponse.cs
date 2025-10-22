using ChatBot.bot.services.ai.data.clients.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Google.Response;

public class VertexAiResponse {
    [JsonProperty("candidates")]
    public List<Candidate>? Candidates { get; set; }
}