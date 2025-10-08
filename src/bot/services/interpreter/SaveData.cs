using ChatBot.bot.interfaces;
using ChatBot.bot.services.interpreter.data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.interpreter;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("vars")]
    public Dictionary<string, StoredVariable> Variables { get; set; }


    public SaveData() {
        Variables = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("vars")] Dictionary<string, StoredVariable> vars) {
        ServiceState = serviceState;
        Variables = vars;
    }
}