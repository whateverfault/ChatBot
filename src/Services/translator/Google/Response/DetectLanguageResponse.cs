using ChatBot.Services.translator.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.Services.translator.Google.Response;

public class DetectLanguageResponse {
    [JsonProperty("languages")]
    public List<DetectedLanguage> Languages { get; set; } = null!;
}