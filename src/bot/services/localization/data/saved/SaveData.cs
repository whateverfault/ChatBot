using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.localization.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("lang")]
    public Lang Language { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("lang")] Lang lang) {
        var dto = new SaveDataDto(
                                  state,
                                  lang
                                  );
        FromDto(dto);
    }
    
    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Language = dto.Language.Value;
    }
}