using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.logger;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; private set; }
    [JsonProperty(PropertyName = "non_twitch_logs")]
    public List<Log> Logs { get; private set; }
    [JsonProperty(PropertyName = "twitch_logs")]
    public List<Log> TwitchLogs { get; private set; }


    public SaveData(State serviceState) {
        ServiceState = serviceState;
        Logs = [];
        TwitchLogs = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "logs")] List<Log> logs,
        [JsonProperty(PropertyName = "twitch_logs")] List<Log> twitchLogs
    ) {
        ServiceState = serviceState;
        Logs = logs;
        TwitchLogs = twitchLogs;
    }

    public State GetState() {
        return ServiceState;
    }
    
    public void SetState(State state) {
        ServiceState = state;
    }
}