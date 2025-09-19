using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.casino;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("random_val")]
    public float RandomValue { get; set; }
    
    [JsonProperty("base")]
    public float BaseMultiplier { get; set; }
    
    [JsonProperty("additional")]
    public float AdditionalMultiplier { get; set; }
    

    public SaveData() {
        BaseMultiplier = 1.51f;
        AdditionalMultiplier = 1.3f;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("random_val")] float randomValue,
        [JsonProperty("base")] float baseMultiplier,
        [JsonProperty("additional")] float additional) {
        ServiceState = state;
        RandomValue = randomValue;
        BaseMultiplier = baseMultiplier;
        AdditionalMultiplier = additional;
    }
}