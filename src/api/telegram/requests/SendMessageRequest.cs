using Newtonsoft.Json;

namespace ChatBot.api.telegram.requests;

public class SendMessageRequest {
    [JsonProperty("message")]
    public string Message { get; private set; }
    
    [JsonProperty("chat_id")]
    public long ChatId { get; private set; }
    
    [JsonProperty("disable_notification")]
    public bool DisableNotification { get; private set; }


    public SendMessageRequest(string message, long chatId, bool disableNotification = false) {
        Message = message;
        ChatId = chatId;
        DisableNotification = disableNotification;
    }
}