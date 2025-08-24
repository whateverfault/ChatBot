using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.logger;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty(PropertyName = "logs")]
    public List<Log> Logs { get; set; }


    public SaveData() {
        Logs = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "logs")] List<Log> logs) {
        ServiceState = serviceState;
        Logs = logs;
    }
}