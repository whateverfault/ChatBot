using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.presets.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);

    public readonly SafeField<List<Preset>> Presets = new SafeField<List<Preset>>([]);

    public readonly SafeField<int> CurrentPreset = new SafeField<int>(0);


    public SaveDataDto() {
        var preset = Preset.Default;
        preset.Create();
        
        Presets.Value.Add(preset);
    }
    
    public SaveDataDto(
        State serviceState,
        List<Preset> presets,
        int currentPreset) {
        ServiceState.Value = serviceState;
        Presets.Value = presets;
        CurrentPreset.Value = currentPreset;
    }
}