using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.bot_lifecycle.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);
    public readonly SafeField<long> DisconnectTimeout = new SafeField<long>(1800);
    

    public SaveDataDto() { }
    
    public SaveDataDto(
        State state, 
        long disconnectCooldown) {
        ServiceState.Value = state;
        DisconnectTimeout.Value = disconnectCooldown;
    }
}