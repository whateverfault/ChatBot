using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.moderation.data.saved;

internal class SaveData {
    [JsonProperty("service_state")] 
    public State ServiceState { get; set; }
    
    [JsonProperty("moderation_actions")]
    public List<ModAction> ModerationActions { get; set; } = null!;

    [JsonProperty("warned_users")]
    public List<WarnedUser> WarnedUsers { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("moderation_actions")] List<ModAction> actions,
        [JsonProperty("warned_users")] List<WarnedUser> warnedUsers) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  actions,
                                  warnedUsers
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        ModerationActions = dto.ModerationActions.Value;
        WarnedUsers = dto.WarnedUsers.Value;
    }
}