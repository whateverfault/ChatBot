using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.calculator;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    
    public SaveData() { }
    
    [JsonConstructor]
    public SaveData([JsonProperty("service_state")] State state) {
        ServiceState = state;
    }
}