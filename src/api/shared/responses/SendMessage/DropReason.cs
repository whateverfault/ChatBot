using Newtonsoft.Json;

namespace ChatBot.api.shared.responses.SendMessage;

public class DropReason {
    [JsonProperty("code")]
    public string Code { get; private set; }
    
    [JsonProperty("message")]
    public string Message { get; private set; }
}