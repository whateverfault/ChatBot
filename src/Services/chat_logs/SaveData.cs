using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_logs;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "logs")]
    public List<Message> Logs { get; private set; }


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "logs")] List<Message> logs) {
        ServiceState = serviceState;
        Logs = logs;
    }
}