using Newtonsoft.Json;

namespace ChatBot.bot.utils.Telegram.Response.Data;

public class SendMessageResult {
    [JsonProperty("message_id")]
    public int MessageId { get; set; }
}