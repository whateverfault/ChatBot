using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.bot_lifecycle.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("disconnect_timeout")]
    public long DisconnectTimeout { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("disconnect_timeout")] long disconnectTimeout) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  disconnectTimeout
                                 );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        DisconnectTimeout = dto.DisconnectTimeout.Value;
    }
}