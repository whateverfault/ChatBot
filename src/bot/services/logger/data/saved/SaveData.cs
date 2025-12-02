using ChatBot.bot.interfaces;
using Newtonsoft.Json;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("log_level")]
    public LogLevel LogLevel { get; set; }
    
    [JsonProperty("logs")]
    public List<Log> Logs { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("log_level")] LogLevel logLevel,
        [JsonProperty("logs")] List<Log> logs) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  logLevel,
                                  logs
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        LogLevel = dto.LogLevel.Value;
        Logs = dto.Logs.Value;
    }
}