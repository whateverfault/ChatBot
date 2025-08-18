using Newtonsoft.Json;

namespace ChatBot.api.telegram.response.Data;

public class SendMessageResult {
    [JsonProperty("message_id")]
    public int MessageId { get; set; }
}