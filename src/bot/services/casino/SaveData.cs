using ChatBot.bot.interfaces;
using ChatBot.bot.services.casino.data;
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

    [JsonProperty("duels")]
    public List<Duel> Duels { get; set; }


    public SaveData() {
        Duels = [];
        BaseMultiplier = 1.51f;
        AdditionalMultiplier = 1.3f;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("random_val")] float randomValue,
        [JsonProperty("base")] float baseMultiplier,
        [JsonProperty("additional")] float additional,
        [JsonProperty("duels")] List<Duel> duels) {
        ServiceState = state;
        RandomValue = randomValue;
        BaseMultiplier = baseMultiplier;
        AdditionalMultiplier = additional;
        Duels = duels;
    }
}