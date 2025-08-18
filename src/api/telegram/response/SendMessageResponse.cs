using ChatBot.api.telegram.response.Data;
using Newtonsoft.Json;

namespace ChatBot.api.telegram.response;

public class SendMessageResponse {
    [JsonProperty("ok")]
    public bool Ok { get; set; }
    
    [JsonProperty("result")]
    public SendMessageResult Result { get; set; } = null!;
}