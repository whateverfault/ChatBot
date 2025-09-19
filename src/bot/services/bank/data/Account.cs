using Newtonsoft.Json;

namespace ChatBot.bot.services.bank.data;

public class Account {
    [JsonProperty("user_id")]
    public string UserId { get; private set; }
    
    [JsonProperty("money")]
    public long Money { get; private set; }
    
    [JsonProperty("gain")]
    public long Gain { get; private set; }

    
    public Account(string userId, long money = 0) {
        UserId = userId;
        Money = money;
    }
    
    [JsonConstructor]
    public Account(
        [JsonProperty("user_id")] string userId,
        [JsonProperty("money")] long money,
        [JsonProperty("gain")] long gain) {
        UserId = userId;
        Money = money;
        Gain = gain;
    }
    
    public void AddMoney(long quantity, bool gain = false) {
        Money += quantity;
        if (gain) Gain += quantity;
    }
}