using ChatBot.bot.services.translator.Google.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.Google.Response;

public class TranslateTextResponse {
    [JsonProperty("translations")]
    public List<Translation>? Translations { get; set; }
}