using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop.data;

public class ShopLot {
    private static ShopService Shop => (ShopService)ServiceManager.GetService(ServiceName.Shop);
    
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
        Shop.Options.Save();
    }
    
    public void ChangeCost(long cost) {
        Cost = cost;
        Shop.Options.Save();
    }
}