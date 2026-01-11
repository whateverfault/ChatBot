using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.demon_list.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);
    
    public readonly SafeField<string> DefaultUserName = new SafeField<string>(string.Empty);
    
    public readonly SafeField<bool> UseDefaultUserName = new SafeField<bool>(false);
    
    
    public SaveDataDto() {}
    
    public SaveDataDto(
        State serviceState,
        string defaultUserName,
        bool useDefaultUserName) {
        ServiceState.Value = serviceState;
        DefaultUserName.Value = defaultUserName;
        UseDefaultUserName.Value = useDefaultUserName;
    }
}