using ChatBot.services.translator.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.services.translator.Google.Response;

public class TranslateTextResponse {
    [JsonProperty("translations")]
    public List<Translation>? Translations { get; set; }
}