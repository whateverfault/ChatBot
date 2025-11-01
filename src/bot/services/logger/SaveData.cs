using ChatBot.bot.interfaces;
using Newtonsoft.Json;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("log_level")]
    public LogLevel LogLevel { get; set; }
    
    [JsonProperty("logs")]
    public List<Log> Logs { get; set; }


    public SaveData() {
        LogLevel = LogLevel.Warning;
        Logs = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("log_level")] LogLevel logLevel,
        [JsonProperty("logs")] List<Log> logs) {
        ServiceState = serviceState;
        LogLevel = logLevel;
        Logs = logs;
    }
}