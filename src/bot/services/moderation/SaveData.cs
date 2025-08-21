using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;

namespace ChatBot.bot.services.moderation;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")] 
    public State ServiceState { get; set; }
    
    [JsonProperty(PropertyName ="moderation_actions")]
    public List<ModAction> ModerationActions { get; set; }

    [JsonProperty(PropertyName = "warned_users")]
    public List<WarnedUser> WarnedUsers { get; set; }


    public SaveData() {
        ModerationActions = [
                                new ModAction(
                                              "Level Requests",
                                              0,
                                              600,
                                              "Реквесты только для випов и выше",
                                              2,
                                              Restriction.Vip,
                                              true
                                             ),
                            ];
        WarnedUsers = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName ="service_state")]  State serviceState,
        [JsonProperty(PropertyName ="moderation_actions")] List<ModAction> actions,
        [JsonProperty(PropertyName = "warned_users")] List<WarnedUser> warnedUsers) {
        ServiceState = serviceState;
        ModerationActions = actions;
        WarnedUsers = warnedUsers;
    }
}