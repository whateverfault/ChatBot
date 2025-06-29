using ChatBot.Services.ai.AiClients.interfaces;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.ai;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "ai_kind")]
    public AiKind AiKind { get; set; }

    [JsonProperty(PropertyName = "ai_data")]
    public List<AiData> AiData { get; set; }
    
    [JsonProperty(PropertyName = "google_project_id")]
    public string GoogleProjectId { get; set; } = null!;


    public SaveData(List<AiData> aiData) {
        AiData = aiData;
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "ai_mode")] AiKind aiKind,
        [JsonProperty(PropertyName = "ai_data")] List<AiData> aiData,
        [JsonProperty(PropertyName = "google_project_id")] string googleProjectId) {
        ServiceState = serviceState;
        AiKind = aiKind;
        AiData = aiData;
        GoogleProjectId = googleProjectId;
    }
}