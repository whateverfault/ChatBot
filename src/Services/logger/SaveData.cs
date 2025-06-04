using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.logger;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; private set; }
    [JsonProperty(PropertyName = "logs")]
    public List<Log> Logs { get; private set; }


    public SaveData(State serviceState) {
        ServiceState = serviceState;
        Logs = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "logs")] List<Log> logs
        ) {
        ServiceState = serviceState;
        Logs = logs;
    }

    public State GetState() {
        return ServiceState;
    }
    
    public void SetState(State state) {
        ServiceState = state;
    }
}