using ChatBot.services.translator.VK.Data;
using Newtonsoft.Json;

namespace ChatBot.services.translator.VK.Response;

public class VkTranslateResponse {
    [JsonProperty("response")]
    public VkTranslateInfo? Response { get; set; }
}