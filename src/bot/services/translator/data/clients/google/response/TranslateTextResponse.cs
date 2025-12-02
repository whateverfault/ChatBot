using ChatBot.bot.services.translator.data.clients.google.data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.data.clients.Google.response;

public class TranslateTextResponse {
    [JsonProperty("translations")]
    public List<Translation>? Translations { get; set; }
}