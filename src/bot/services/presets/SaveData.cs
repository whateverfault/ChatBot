using ChatBot.bot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.presets;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty(PropertyName = "presets")]
    public List<Preset> Presets { get; set; }
    
    [JsonProperty(PropertyName = "current_preset")]
    public int CurrentPreset { get; set; }
    

    public SaveData() {
        var preset = new Preset("default", 0);
        preset.Create();
        
        Presets = [preset,];
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