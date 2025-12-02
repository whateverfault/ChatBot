using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.demon_list.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);
    
    
    public SaveDataDto() {}
    
    public SaveDataDto(
        State serviceState) {
        ServiceState.Value = serviceState;
    }
}