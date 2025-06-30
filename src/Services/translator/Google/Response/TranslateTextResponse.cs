using ChatBot.Services.translator.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.Services.translator.Google.Response;

public class TranslateTextResponse {
    [JsonProperty("translations")]
    public List<Translation>? Translations { get; set; }
}