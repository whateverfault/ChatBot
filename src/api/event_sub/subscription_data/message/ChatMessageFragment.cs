using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class ChatMessageFragment {
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
}