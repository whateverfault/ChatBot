using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.presets;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "presets")]
    public List<Preset> Presets { get; set; }
    [JsonProperty(PropertyName = "current_preset")]
    public int CurrentPreset { get; set; }
    

    public SaveData(List<Preset> presets) {
        Presets = presets;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "presets")] List<Preset> presets,
        [JsonProperty(PropertyName = "current_preset")] int currentPreset) {
        ServiceState = serviceState;
        Presets = presets;
        CurrentPreset = currentPreset;
    }
}