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

    [JsonProperty(PropertyName = "hf_token")]
    public string HfToken { get; set; } = null!;

    [JsonProperty(PropertyName = "ai_mode")]
    public AiMode AiMode { get; set; }


    public SaveData() {}

    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "model")] string model,
        [JsonProperty(PropertyName = "base_prompt")] string basePrompt,
        [JsonProperty(PropertyName = "hf_token")] string hfToken,
        [JsonProperty(PropertyName = "ai_mode")] AiMode aiMode) {
        ServiceState = serviceState;
        Model = model;
        BasePrompt = basePrompt;
        HfToken = hfToken;
        AiMode = aiMode;
    }
}