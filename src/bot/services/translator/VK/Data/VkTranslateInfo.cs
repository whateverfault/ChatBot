using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.VK.Data;

public class VkTranslateInfo {
    [JsonProperty("texts")]
    public string[] Text { get; set; }
    
    [JsonProperty("source_lang")]
    public string Language { get; set; }


    [JsonConstructor]
    public VkTranslateInfo(
        [JsonProperty("texts")] string[] text,
        [JsonProperty("source_lang")] string language) {
        Text = text;
        Language = language;
    }
}