using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.chat_logs.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);

    public readonly SafeField<List<Message>> Logs = new SafeField<List<Message>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        List<Message> logs) {
        ServiceState.Value = serviceState;
        Logs.Value = logs;
    }
}