using ChatBot.services.translator.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.services.translator.Google.Response;

public class DetectLanguageResponse {
    [JsonProperty("languages")]
    public List<DetectedLanguage> Languages { get; set; } = null!;
}