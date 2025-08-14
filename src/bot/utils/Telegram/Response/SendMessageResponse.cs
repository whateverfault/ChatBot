using ChatBot.bot.utils.Telegram.Response.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.Telegram.Response;

public class SendMessageResponse {
    [JsonProperty("ok")]
    public bool Ok { get; set; }
    
    [JsonProperty("result")]
    public SendMessageResult Result { get; set; } = null!;
}