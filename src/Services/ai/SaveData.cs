using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.ai;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }

    [JsonProperty(PropertyName = "model")]
    public string Model { get; set; } = null!;

    [JsonProperty(PropertyName = "base_prompt")]
    public string LocalPrompt { get; set; } = null!;

    [JsonProperty(PropertyName = "hf_token")]
    public string HfToken { get; set; } = null!;

    [JsonProperty(PropertyName = "ai_mode")]
    public AiMode AiMode { get; set; }

    [JsonProperty(PropertyName = "hf_provider")]
    public string HfProvider { get; set; } = null!;

    [JsonProperty(PropertyName = "hf_model")]
    public string HfModel { get; set; } = null!;

    [JsonProperty(PropertyName = "hf_prompt")]
    public string HfPrompt { get; set; } = null!;

    [JsonProperty(PropertyName = "local_ai_fallback")]
    public State LocalAiFallback { get; set; }


    public SaveData() {}

    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "model")] string model,
        [JsonProperty(PropertyName = "base_prompt")] string localPrompt,
        [JsonProperty(PropertyName = "hf_token")] string hfToken,
        [JsonProperty(PropertyName = "ai_mode")] AiMode aiMode,
        [JsonProperty(PropertyName = "hf_provider")] string hfProvider,
        [JsonProperty(PropertyName = "hf_model")] string hfModel,
        [JsonProperty(PropertyName = "hf_prompt")] string hfPrompt,
        [JsonProperty(PropertyName = "local_ai_fallback")] State fallback) {
        ServiceState = serviceState;
        Model = model;
        LocalPrompt = localPrompt;
        HfToken = hfToken;
        AiMode = aiMode;
        HfProvider = hfProvider;
        HfModel = hfModel;
        HfPrompt = hfPrompt;
        LocalAiFallback = fallback;
    }
}