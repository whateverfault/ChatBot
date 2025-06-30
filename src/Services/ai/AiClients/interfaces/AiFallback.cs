using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.ai.AiClients.interfaces;

public class AiFallback {
    [JsonProperty("state")]
    public State FallbackState { get; set; }
    [JsonProperty("ai")]
    public AiKind FallbackAi { get; set; }


    [JsonConstructor]
    public AiFallback(
        [JsonProperty("state")] State state,
        [JsonProperty("ai")] AiKind aiKind) {
        FallbackState = state;
        FallbackAi = aiKind;
    }
}