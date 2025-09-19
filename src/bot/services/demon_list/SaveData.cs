using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.demon_list;

internal class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    
    
    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState) {
        ServiceState = serviceState;
    }
}