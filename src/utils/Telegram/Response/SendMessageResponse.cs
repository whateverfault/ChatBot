using ChatBot.utils.Telegram.Response.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.Telegram.Response;

public class SendMessageResponse {
    [JsonProperty("ok")]
    public bool Ok { get; set; }
    
    [JsonProperty("result")]
    public SendMessageResult Result { get; set; }
}