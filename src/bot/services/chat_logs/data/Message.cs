using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_logs.data;

public class Message {
    public static readonly Message Empty = new Message(string.Empty, string.Empty);
    
    
    [JsonProperty("text")]
    public string Text { get; }
    
    [JsonProperty("user_id")]
    public string UserId { get; }
    
    
    [JsonConstructor]
    public Message(
        [JsonProperty("text")] string text,
        [JsonProperty("user_id")] string userId) {
        Text = text;
        UserId = userId;
    }
}