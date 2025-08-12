using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.Google.Data;

public class Translation {
    [JsonProperty("translatedText")]
    public string? TranslatedText { get; set; }
    
    [JsonProperty("model")]
    public string? Model { get; set; }
    
    [JsonProperty("detectedLanguageCode")]
    public string? DetectedLanguageCode { get; set; }
}