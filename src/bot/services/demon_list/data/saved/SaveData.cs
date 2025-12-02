using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.demon_list.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState) {
        var dto = new SaveDataDto(
                                  serviceState
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
    }
}