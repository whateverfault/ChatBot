using ChatBot.api.twitch.helix;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_logs;

public class Message {
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