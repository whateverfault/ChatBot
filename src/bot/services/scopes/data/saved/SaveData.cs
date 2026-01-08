using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.scopes.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("scopes_preset")]
    public ScopesPreset Preset { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("scopes_preset")] ScopesPreset preset) {
        var dto = new SaveDataDto(
                                  state,
                                  preset
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Preset = dto.Preset.Value;
    }
}