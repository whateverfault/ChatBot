using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("project_id")]
    public string ProjectId { get; set; } = null!;

    [JsonProperty("location")]
    public string Location { get; set; } = null!;

    [JsonProperty("google_token")]
    public string GoogleToken { get; set; } = null!;
    
    [JsonProperty("vk_token")]
    public string VkToken { get; set; } = null!;

    [JsonProperty("target_language")]
    public string TargetLanguage { get; set; } = null!;

    [JsonProperty("service")]
    public TranslationService TranslationService { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("project_id")] string projectId,
        [JsonProperty("location")] string location,
        [JsonProperty("google_token")] string googleToken,
        [JsonProperty("vk_token")] string vkToken,
        [JsonProperty("target_language")] string targetLanguage,
        [JsonProperty("service")] TranslationService service) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  projectId,
                                  location,
                                  googleToken,
                                  vkToken,
                                  targetLanguage,
                                  service
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        ProjectId = dto.ProjectId.Value;
        Location = dto.Location.Value;
        GoogleToken = dto.GoogleToken.Value;
        VkToken = dto.VkToken.Value;
        TargetLanguage = dto.TargetLanguage.Value;
        TranslationService = dto.TranslationService.Value;
    }
}