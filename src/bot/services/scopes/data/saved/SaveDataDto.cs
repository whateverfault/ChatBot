using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.scopes.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);
    public readonly SafeField<ScopesPreset> Preset = new SafeField<ScopesPreset>(ScopesPreset.Bot);
    
    
    public SaveDataDto() { }
    
    public SaveDataDto(State state, ScopesPreset preset) {
        ServiceState.Value = state;
        Preset.Value = preset;
    }
}