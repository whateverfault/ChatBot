using Newtonsoft.Json;

namespace ChatBot.bot.services.bank.data;

public class Account {
    [JsonProperty("user_id")]
    public string UserId { get; private set; }
    
    [JsonProperty("money")]
    public long Money { get; private set; }
    
    [JsonProperty("gain")]
    public long Gain { get; private set; }

    [JsonProperty("last_active")]
    public long LastActive { get; private set; }
    
    
    public Account(string userId, long money = 0) {
        UserId = userId;
        Money = money;
        UpdateActivity();
    }
    
    [JsonConstructor]
    public Account(
        [JsonProperty("user_id")] string userId,
        [JsonProperty("money")] long money,
        [JsonProperty("gain")] long gain,
        [JsonProperty("last_active")] long lastActive) {
        UserId = userId;
        Money = money;
        Gain = gain;
        LastActive = lastActive;
    }
    
    public void AddMoney(long quantity, bool gain = false) {
        Money += quantity;
        if (gain) Gain += quantity;
    }

    public void UpdateActivity() {
        LastActive = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}