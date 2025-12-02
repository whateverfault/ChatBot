using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.presets.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("presets")]
    public List<Preset> Presets { get; set; } = null!;

    [JsonProperty("current_preset")]
    public int CurrentPreset { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("presets")] List<Preset> presets,
        [JsonProperty("current_preset")] int currentPreset) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  presets,
                                  currentPreset
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Presets = dto.Presets.Value;
        CurrentPreset = dto.CurrentPreset.Value;
    }
}