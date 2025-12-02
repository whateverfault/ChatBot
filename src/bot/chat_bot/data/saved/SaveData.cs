using Newtonsoft.Json;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveData {
    [JsonProperty("credentials")]
    public ConnectionCredentials Credentials { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("credentials")] ConnectionCredentials credentials) {
        var dto = new SaveDataDto(credentials);
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        Credentials = dto.Credentials.Value;
    }
}