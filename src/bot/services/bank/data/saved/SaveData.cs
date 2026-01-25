using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.bank.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("accounts")]
    public Dictionary<string, Account> Accounts { get; set; } = null!;

    [JsonProperty("rewards")]
    public Dictionary<string, long> Rewards { get; set; } = null!;

    [JsonProperty("supply")]
    public long MoneySupply { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("accounts")] Dictionary<string, Account> accounts,
        [JsonProperty("rewards")] Dictionary<string, long> rewards,
        [JsonProperty("supply")] long supply) {
        var dto = new SaveDataDto(
                                  state,
                                  accounts,
                                  rewards,
                                  supply
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Accounts = dto.Accounts.Value;
        Rewards = dto.Rewards.Value;
        MoneySupply = dto.MoneySupply.Value;
    }
}