using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.interfaces;

public class AiData {
    [JsonProperty("api_key")]
    public string ApiKey { get; set; }
    [JsonProperty("model")]
    public string Model { get; set;  }
    [JsonProperty("base_prompt")]
    public string BasePrompt { get; set;  }
    [JsonProperty("endpoint")]
    public string Endpoint { get; set; }
    [JsonProperty("provider")]
    public string Provider { get; set; }
    [JsonProperty("fallback")]
    public AiFallback Fallback { get; set;  }
    
    
    [JsonConstructor]
    public AiData(
        [JsonProperty("api_key")] string apiKey,
        [JsonProperty("model")] string model,
        [JsonProperty("base_prompt")] string basePrompt,
        [JsonProperty("endpoint")] string endpoint,
        [JsonProperty("provider")] string provider,
        [JsonProperty("fallback")] AiFallback fallback) {
        ApiKey = apiKey;
        Model = model;
        BasePrompt = basePrompt;
        Endpoint = endpoint;
        Provider = provider;
        Fallback = fallback;
    }
}