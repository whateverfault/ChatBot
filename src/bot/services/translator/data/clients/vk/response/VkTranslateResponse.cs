using ChatBot.bot.services.translator.data.clients.vk.data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.data.clients.vk.response;

public class VkTranslateResponse {
    [JsonProperty("response")]
    public VkTranslateInfo? Response { get; set; }
}