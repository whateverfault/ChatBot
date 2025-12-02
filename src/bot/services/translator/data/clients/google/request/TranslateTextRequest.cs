using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.data.clients.Google.request;

public class TranslateTextRequest {
    [JsonProperty("contents")]
    public List<string> Contents { get; set; } = [];
    
    [JsonProperty("targetLanguageCode")]
    public string? TargetLanguageCode { get; set; }

    [JsonProperty("sourceLanguageCode")]
    public string? SourceLanguageCode { get; set; }

    [JsonProperty("mimeType")]
    public string MimeType { get; set; } = "text/plain";
    
    [JsonProperty("model")]
    public string Model { get; set; } = "base";
}