using ChatBot.bot.services.translator.VK.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.VK.Response;

public class VkTranslateResponse {
    [JsonProperty("response")]
    public VkTranslateInfo? Response { get; set; }
}