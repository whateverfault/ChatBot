using ChatBot.bot.interfaces;
using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.shop.data;

public class ShopLot {
    private static readonly ShopService _shop = (ShopService)ServiceManager.GetService(ServiceName.Shop);
    private static int _id;
    
    [JsonProperty("state")]
    public State State { get; private set; }
    
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("id")]
    public int Id { get; private set; }
    
    [JsonProperty("is_default")]
    public bool IsDefault { get; private set; }
    
    [JsonProperty("cost")]
    public long Cost { get; private set; }
    
    [JsonProperty("buyers")]
    public Dictionary<string, long> Buyers;
    

    public ShopLot(string name, long cost, bool isDefault = false, State state = State.Enabled) {
        Buyers = [];
        Name = name;
        Cost = cost;
        IsDefault = isDefault;
        State = state;
        
        Id = _id++;
    }
    
    [JsonConstructor]
    public ShopLot(
        [JsonProperty("id")] int id,
        [JsonProperty("name")] string name,
        [JsonProperty("cost")] long cost,
        [JsonProperty("is_default")] bool isDefault,
        [JsonProperty("state")] State state,
        [JsonProperty("buyers")] Dictionary<string, long> buyers) {
        Id = id;
        Name = name;
        Cost = cost;
        State = state;
        Buyers = buyers;
        IsDefault = isDefault;

        if (id > _id) _id = Id;
    }

    public string GetName() {
        return Name;
    }
    
    public long GetCost() {
        return Cost;
    }

    public int GetStateAsInt() {
        return (int)State;
    }
    
    public void ChangeName(string name) {
        Name = name;
        _shop.Options.Save();
    }
    
    public void ChangeCost(long cost) {
        Cost = cost;
        _shop.Options.Save();
    }

    public void StateNext() {
        State = State == State.Enabled ? State.Disabled : State.Enabled;
        _shop.Options.Save();
    }
    
    public void AddBuyer(string userId, long amount) {
        if (!Buyers.TryAdd(userId, amount)) {
            Buyers[userId] += amount;
        }
        _shop.Options.Save();
    }
    
    public bool Use(string userId) {
        if (!Buyers.TryGetValue(userId, out var val) || val <= 0) {
            return false;
        }
        
        Buyers[userId] -= 1;
        _shop.Options.Save();
        return true;
    }
}