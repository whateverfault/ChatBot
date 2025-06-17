using Newtonsoft.Json;

namespace ChatBot.Services.ai.Ollama;

public class OllamaResponse {
    [JsonProperty(PropertyName = "response")]
    public string? Response { get; private set; }
    
    [JsonProperty(PropertyName = "done")] 
    public bool Done { get; private set; }
    
    
    public OllamaResponse() {}
    
    [JsonConstructor]
    public OllamaResponse(
        [JsonProperty(PropertyName = "response")] string? response,
        [JsonProperty(PropertyName = "done")] bool done) {
        Response = response;
        Done = done;
    }
}