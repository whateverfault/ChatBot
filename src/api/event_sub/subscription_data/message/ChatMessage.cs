using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class ChatMessage {
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("fragments")]
    public ChatMessageFragment[] Fragments { get; set; }
}