using ChatBot.bot.services.casino;
using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop.data;

public class ShopLot {
    private static CasinoService Casino => (CasinoService)ServiceManager.GetService(ServiceName.Casino);
    
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("cost")]
    public long Cost { get; private set; }


    [JsonConstructor]
    public ShopLot(
        [JsonProperty("name")] string name,
        [JsonProperty("cost")] long cost) {
        Name = name;
        Cost = cost;
    }

    public string GetName() {
        return Name;
    }
    
    public long GetCost() {
        return Cost;
    }
    
    public void ChangeName(string name) {
        Name = name;
        Casino.Options.Save();
    }
    
    public void ChangeCost(long cost) {
        Cost = cost;
        Casino.Options.Save();
    }
}