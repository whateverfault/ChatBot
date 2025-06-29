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

    [JsonProperty(PropertyName = "api_token")]
    public string Token { get; set; } = null!;

    [JsonProperty(PropertyName = "target_language")]
    public string TargetLanguage { get; set; } = null!;


    public SaveData(){}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "project_id")] string projectId,
        [JsonProperty(PropertyName = "location")] string location,
        [JsonProperty(PropertyName = "api_token")] string token,
        [JsonProperty(PropertyName = "target_language")] string targetLanguage) {
        ServiceState = serviceState;
        ProjectId = projectId;
        Location = location;
        Token = token;
        TargetLanguage = targetLanguage;
    }
}