using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.Google.Data;

public class DetectedLanguage {
    [JsonProperty("languageCode")]
    public string? LanguageCode { get; set; }
    
    [JsonProperty("confidence")]
    public float Confidence { get; set; }
}