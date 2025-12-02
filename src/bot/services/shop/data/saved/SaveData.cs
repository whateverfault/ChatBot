using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("lots")]
    public List<ShopLot> Lots { get; private set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("lots")] List<ShopLot> lots) {
        var dto = new SaveDataDto(
                                  state,
                                  lots
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Lots = dto.Lots.Value;
    }
}