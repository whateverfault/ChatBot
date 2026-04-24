using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.bank.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<Dictionary<string, Account>> Accounts = new SafeField<Dictionary<string, Account>>([]);

    public readonly SafeField<Dictionary<string, double>> Rewards = new SafeField<Dictionary<string, double>>([]);

    public readonly SafeField<double> MoneySupply = new SafeField<double>(0);
    
    public readonly SafeField<string> BuyBackReward = new SafeField<string>(string.Empty);
    
    public readonly SafeField<double> PointsForBuyBack = new SafeField<double>(300);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State state,
        Dictionary<string, Account> accounts,
        Dictionary<string, double> rewards,
        double supply,
        string buyBackReward,
        double pointsForBuyBack) {
        ServiceState.Value = state;
        Accounts.Value = accounts;
        Rewards.Value = rewards;
        MoneySupply.Value = supply;
        BuyBackReward.Value = buyBackReward;
        PointsForBuyBack.Value = pointsForBuyBack;
    }
}