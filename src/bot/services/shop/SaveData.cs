using ChatBot.bot.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("lots")]
    public List<ShopLot> Lots{ get; }
    
    
    public SaveData() {
        Lots = [
                   new ShopLot(ServiceName.Ai, 50, isDefault: true, state: State.Disabled),
                   new ShopLot("Mute", 10000, isDefault: true, state: State.Disabled),
                   new ShopLot("AntiMute", 7500, isDefault: true, state: State.Disabled),
               ];
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("lots")] List<ShopLot> shop) {
        ServiceState = state;
        Lots = shop;
    }
}