using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.ai;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }

    [JsonProperty(PropertyName = "model")]
    public string Model { get; set; } = null!;

    [JsonProperty(PropertyName = "base_prompt")]
    public string BasePrompt { get; set; } = null!;


    public SaveData() {}

    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "model")] string model,
        [JsonProperty(PropertyName = "base_prompt")] string basePrompt) {
        ServiceState = serviceState;
        Model = model;
        BasePrompt = basePrompt;
    }
}