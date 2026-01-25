using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.localization.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);
    
    public readonly SafeField<Lang> Language = new SafeField<Lang>(Lang.Eng);
    
    
    public SaveDataDto(){}

    public SaveDataDto(State state, Lang lang) {
        ServiceState.Value = state;
        Language.Value = lang;
    }
}