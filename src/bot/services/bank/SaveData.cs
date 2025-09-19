using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank.data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.bank;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("accounts")]
    public Dictionary<string, Account> Accounts { get; set; }
    
    [JsonProperty("rewards")]
    public Dictionary<string, long> Rewards { get; set; }
    
    [JsonProperty("supply")]
    public long MoneySupply { get; set; }


    public SaveData() {
        Accounts = [];
        Rewards = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("accounts")] Dictionary<string, Account> accounts,
        [JsonProperty("rewards")] Dictionary<string, long> rewards,
        [JsonProperty("supply")] long supply) {
        ServiceState = state;
        Accounts = accounts;
        Rewards = rewards;
        MoneySupply = supply;
    }
}