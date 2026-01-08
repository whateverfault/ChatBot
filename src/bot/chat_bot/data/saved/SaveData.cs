using ChatBot.bot.services.scopes;
using Newtonsoft.Json;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveData {
    [JsonProperty("credentials")]
    public ConnectionCredentials Credentials { get; set; } = null!;

    [JsonProperty("auth_lvl")]
    public ScopesPreset CurAuthLevel { get; set; }
    
    [JsonProperty("has_broad_auth")]
    public bool HasBroadcasterAuth { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("credentials")] ConnectionCredentials credentials,
        [JsonProperty("auth_lvl")] ScopesPreset authLevel,
        [JsonProperty("has_broad_auth")] bool hasBroadAuth) {
        var dto = new SaveDataDto(
                                  credentials,
                                  authLevel,
                                  hasBroadAuth
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        Credentials = dto.Credentials.Value;
        CurAuthLevel = dto.CurAuthLevel.Value;
        HasBroadcasterAuth = dto.HasBroadcasterAuth.Value;
    }
}