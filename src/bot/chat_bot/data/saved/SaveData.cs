using Newtonsoft.Json;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveData {
    [JsonProperty("credentials")]
    public ConnectionCredentials Credentials { get; set; } = null!;

    [JsonProperty("auth_lvl")]
    public AuthLevel AuthLevel { get; set; }
    
    [JsonProperty("authed_creds")]
    public FullCredentials? AuthorizedCredentials { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("credentials")] ConnectionCredentials credentials,
        [JsonProperty("auth_lvl")] AuthLevel authLevel,
        [JsonProperty("authed_creds")] FullCredentials? authedCreds) {
        var dto = new SaveDataDto(
                                  credentials,
                                  authLevel,
                                  authedCreds
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        Credentials = dto.Credentials.Value;
        AuthLevel = dto.AuthenticationLevel.Value;
        AuthorizedCredentials = dto.AuthorizedCredentials.Value;
    }
}