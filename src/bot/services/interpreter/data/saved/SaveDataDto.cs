using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.interpreter.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);
    
    public readonly SafeField<Dictionary<string, StoredVariable>> Variables = new SafeField<Dictionary<string, StoredVariable>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        Dictionary<string, StoredVariable> vars) {
        ServiceState.Value = serviceState;
        Variables.Value = vars;
    }
}