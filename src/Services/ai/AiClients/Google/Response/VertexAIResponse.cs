using ChatBot.services.ai.AiClients.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.services.ai.AiClients.Google.Response;

public class VertexAiResponse {
    [JsonProperty("candidates")]
    public List<Candidate>? Candidates { get; set; }
}