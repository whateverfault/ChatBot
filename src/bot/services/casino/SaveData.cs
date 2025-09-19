using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.casino;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("random_val")]
    public float RandomValue { get; set; }
    

    public SaveData() {
        ServiceState = State.Disabled;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("random_val")] float randomValue) {
        ServiceState = state;
        RandomValue = randomValue;
    }
}