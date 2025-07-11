using System.Security.Principal;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.translator;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "project_id")]
    public string ProjectId { get; set; } = null!;

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; } = null!;

    [JsonProperty(PropertyName = "google_token")]
    public string GoogleToken { get; set; } = null!;
    
    [JsonProperty(PropertyName = "vk_token")]
    public string VkToken { get; set; } = null!;

    [JsonProperty(PropertyName = "target_language")]
    public string TargetLanguage { get; set; } = null!;

    [JsonProperty(PropertyName = "service")]
    public TranslationService TranslationService { get; set; }


    public SaveData(){}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "project_id")] string projectId,
        [JsonProperty(PropertyName = "location")] string location,
        [JsonProperty(PropertyName = "google_token")] string googleToken,
        [JsonProperty(PropertyName = "vk_token")] string vkToken,
        [JsonProperty(PropertyName = "target_language")] string targetLanguage,
        [JsonProperty(PropertyName = "service")] TranslationService service) {
        ServiceState = serviceState;
        ProjectId = projectId;
        Location = location;
        GoogleToken = googleToken;
        VkToken = vkToken;
        TargetLanguage = targetLanguage;
        TranslationService = service;
    }
}