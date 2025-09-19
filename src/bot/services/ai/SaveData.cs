using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai.AiClients.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("ai_kind")]
    public AiKind AiKind { get; set; }

    [JsonProperty("ai_data")]
    public List<AiData> AiData { get; set; }
    
    [JsonProperty("google_project_id")]
    public string GoogleProjectId { get; set; }

    [JsonProperty("casino_integration")]
    public State CasinoIntegration { get; set; }


    public SaveData() {
        GoogleProjectId = string.Empty;
        CasinoIntegration = State.Disabled;
        
        AiData = [
                     new AiData(
                             "Empty",
                             "Empty",
                             "Empty",
                             "http://localhost:11434/api/generate",
                             "Empty",
                             new AiFallback(State.Disabled, AiKind.Ollama)
                            ),
                     new AiData(
                                "Empty",
                                "Empty",
                                "Empty",
                                "https://router.huggingface.co/Empty/v1/chat/completions",
                                "Empty",
                                new AiFallback(State.Disabled, AiKind.Ollama)
                               ),
                     new AiData(
                                "Empty",
                                "deepseek-chat",
                                "Empty",
                                "https://api.deepseek.com/chat/completions",
                                "Empty",
                                new AiFallback(State.Disabled, AiKind.Ollama)
                               ),
                     new AiData(
                                "Empty",
                                "publishers/google/models/gemini-pro",
                                "Empty",
                                "https://aiplatform.googleapis.com/v1/projects/{projectId}/locations/{location}/{model}:generateContent\"",
                                "Empty",
                                new AiFallback(State.Disabled, AiKind.Ollama)
                               ),
                 ];
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "ai_mode")] AiKind aiKind,
        [JsonProperty(PropertyName = "ai_data")] List<AiData> aiData,
        [JsonProperty(PropertyName = "google_project_id")] string googleProjectId,
        [JsonProperty("casino_integration")] State casinoIntegration) {
        ServiceState = serviceState;
        AiKind = aiKind;
        AiData = aiData;
        GoogleProjectId = googleProjectId;
        CasinoIntegration = casinoIntegration;
    }
}