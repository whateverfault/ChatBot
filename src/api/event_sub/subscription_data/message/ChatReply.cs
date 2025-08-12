using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class ChatReply {
    [JsonProperty("parent_message_id")]
    public string MessageId { get; set; }
    
    [JsonProperty("parent_message_body")]
    public string Text { get; set; }
    
    [JsonProperty("parent_user_id")]
    public string UserId { get; set; }
    
    [JsonProperty("parent_user_login")]
    public string Username { get; set; }
}