using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.moderation;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")] 
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName ="moderation_actions")]
    public List<ModAction> ModerationActions { get; set; } = null!;

    [JsonProperty(PropertyName = "warned_users")]
    public List<WarnedUser> WarnedUsers { get; set; } = null!;


    public SaveData() {}
    
    public SaveData(
        [JsonProperty(PropertyName ="service_state")]  State serviceState,
        [JsonProperty(PropertyName ="moderation_actions")] List<ModAction> actions,
        [JsonProperty(PropertyName = "warned_users")] List<WarnedUser> warnedUsers) {
        ServiceState = serviceState;
        ModerationActions = new List<ModAction>(actions);
        WarnedUsers = warnedUsers;
    }
}