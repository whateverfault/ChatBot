using ChatBot.Services.translator.VK.Data;
using Newtonsoft.Json;

namespace ChatBot.Services.translator.VK.Response;

public class VkTranslateResponse {
    [JsonProperty("response")]
    public VkTranslateInfo? Response { get; set; }
}