using Newtonsoft.Json;

namespace ChatBot.api.shared.responses.SendMessage;

public class SendMessageResponse {
    [JsonProperty("data")]
    public SentMessage[] Data { get; private set; }
}