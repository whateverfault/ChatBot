using ChatBot.bot.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("lots")]
    public ShopLot[] Lots{ get; }
    
    
    public SaveData() {
        Lots = [
                   new ShopLot(ServiceName.Ai, 0),
               ];
    }

    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("lots")] ShopLot[] shop) {
        ServiceState = state;
        Lots = shop;
    }
}