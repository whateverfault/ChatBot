using ChatBot.bot.interfaces;
using Newtonsoft.Json;
using TwitchAPI.api.data.responses.GetUserInfo;

namespace ChatBot.bot.services.message_filter.data.saved;

internal class SaveData {
    [JsonProperty("state")]
    public State State { get; set; }
    
    [JsonProperty("filters")]
    public List<Filter> Filters { get; private set; } = null!;

    [JsonProperty("banned_users")]
    public List<UserInfo> BannedUsers { get; private set; } = null!;
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("filters")] List<Filter> filters,
        [JsonProperty("banned_users")] List<UserInfo> bannedUsers,
        [JsonProperty("state")] State state) {
        var dto = new SaveDataDto(
                                  filters,
                                  bannedUsers,
                                  state
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        Filters = dto.Filters.Value;
        BannedUsers = dto.BannedUsers.Value;
        State = dto.State.Value;
    }
}