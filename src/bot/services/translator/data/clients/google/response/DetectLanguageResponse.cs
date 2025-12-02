using ChatBot.bot.services.translator.data.clients.google.data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.data.clients.Google.response;

public class DetectLanguageResponse {
    [JsonProperty("languages")]
    public List<DetectedLanguage> Languages { get; set; } = null!;
}