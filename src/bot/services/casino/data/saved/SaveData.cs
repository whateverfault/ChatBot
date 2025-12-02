using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.casino.data.saved;

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
    public List<Duel> Duels { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("random_val")] float randomValue,
        [JsonProperty("base")] float baseMultiplier,
        [JsonProperty("additional")] float additional,
        [JsonProperty("duels")] List<Duel> duels) {
        var dto = new SaveDataDto(
                                  state,
                                  randomValue,
                                  baseMultiplier,
                                  additional,
                                  duels
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        RandomValue = dto.RandomValue.Value;
        BaseMultiplier = dto.BaseMultiplier.Value;
        AdditionalMultiplier = dto.AdditionalMultiplier.Value;
        Duels = dto.Duels.Value;
    }
}