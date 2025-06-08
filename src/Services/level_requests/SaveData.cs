using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.level_requests;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "pattern_index")]
    public int PatternIndex { get; set; }
    [JsonProperty(PropertyName = "restriction")]
    public Restriction Restriction { get; set; }


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "pattern_index")] int patternIndex,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction
        ) {
        ServiceState = serviceState;
        PatternIndex = patternIndex;
        Restriction = restriction;
    }
}