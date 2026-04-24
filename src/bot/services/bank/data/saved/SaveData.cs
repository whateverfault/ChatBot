using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.bank.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("accounts")]
    public Dictionary<string, Account> Accounts { get; set; } = null!;

    [JsonProperty("rewards")]
    public Dictionary<string, double> Rewards { get; set; } = null!;

    [JsonProperty("supply")]
    public double MoneySupply { get; set; }

    [JsonProperty("buyback_reward")]
    public string BuyBackReward { get; set; } = null!;
    
    [JsonProperty("points_for_buyback")]
    public double PointsForBuyBack { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("accounts")] Dictionary<string, Account> accounts,
        [JsonProperty("rewards")] Dictionary<string, double> rewards,
        [JsonProperty("supply")] double supply,
        [JsonProperty("buyback_reward")] string buyBackReward,
        [JsonProperty("points_for_buyback")] double pointsForBuyBack) {
        var dto = new SaveDataDto(
                                  state,
                                  accounts,
                                  rewards,
                                  supply,
                                  buyBackReward,
                                  pointsForBuyBack
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Accounts = dto.Accounts.Value;
        Rewards = dto.Rewards.Value;
        MoneySupply = dto.MoneySupply.Value;
        BuyBackReward = dto.BuyBackReward.Value;
        PointsForBuyBack = dto.PointsForBuyBack.Value;
    }
}