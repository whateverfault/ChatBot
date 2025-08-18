using Newtonsoft.Json;

namespace ChatBot.api.telegram.requests;

public class DeleteMessageRequest {
    [JsonProperty("message_id")]
    public long MessageId { get; private set; }
    
    [JsonProperty("chat_id")]
    public long ChatId { get; private set; }


    public DeleteMessageRequest(long messageId, long chatId) {
        MessageId = messageId;
        ChatId = chatId;
    }
}