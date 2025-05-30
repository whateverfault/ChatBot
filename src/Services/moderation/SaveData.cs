using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.moderation;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")] 
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName ="moderation_actions")]
    public List<ModAction> ModerationActions { get; set; }
    [JsonProperty(PropertyName = "warned_users")]
    public List<WarnedUser> WarnedUsers { get; set; }


    public SaveData() {}
    
    public SaveData(State serviceState, List<ModAction> actions, List<WarnedUser> warnedUsers) {
        ServiceState = serviceState;
        ModerationActions = new List<ModAction>(actions);
        WarnedUsers = warnedUsers;
    }
}