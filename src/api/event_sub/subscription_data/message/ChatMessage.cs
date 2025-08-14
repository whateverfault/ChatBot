using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class ChatMessage {
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("fragments")]
    public ChatMessageFragment[] Fragments { get; set; }
    
    
    [JsonConstructor]
    public ChatMessage(
        [JsonProperty("text")] string text,
        [JsonProperty("fragments")] ChatMessageFragment[] fragments) {
        Text = text;
        Fragments = fragments;
    }
}