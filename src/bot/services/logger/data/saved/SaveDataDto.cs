using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<LogLevel> LogLevel = new SafeField<LogLevel>(TwitchAPI.client.LogLevel.Warning);

    public readonly SafeField<List<Log>> Logs = new SafeField<List<Log>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        LogLevel logLevel,
        List<Log> logs) {
        ServiceState.Value = serviceState;
        LogLevel.Value = logLevel;
        Logs.Value = logs;
    }
}