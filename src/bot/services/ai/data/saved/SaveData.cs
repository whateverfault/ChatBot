using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("ai_kind")]
    public AiKind AiKind { get; set; }

    [JsonProperty("ai_data")]
    public List<AiData> AiData { get; set; } = null!;

    [JsonProperty("google_project_id")]
    public string GoogleProjectId { get; set; } = null!;

    [JsonProperty("remove_chat_in")]
    public long RemoveChatIn { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("ai_mode")] AiKind aiKind,
        [JsonProperty("ai_data")] List<AiData> aiData,
        [JsonProperty("google_project_id")] string googleProjectId,
        [JsonProperty("remove_chat_in")] long removeChatIn) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  aiKind,
                                  aiData,
                                  googleProjectId,
                                  removeChatIn
                                  );

        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        AiKind = dto.AiKind.Value;
        AiData = dto.AiData.Value;
        GoogleProjectId = dto.GoogleProjectId.Value;
        RemoveChatIn = dto.RemoveChatIn.Value;
    }
}