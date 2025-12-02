using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.bank.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<Dictionary<string, Account>> Accounts = new SafeField<Dictionary<string, Account>>([]);

    public readonly SafeField<Dictionary<string, long>> Rewards = new SafeField<Dictionary<string, long>>([]);

    public readonly SafeField<long> MoneySupply = new SafeField<long>(0);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State state,
        Dictionary<string, Account> accounts,
        Dictionary<string, long> rewards,
        long supply) {
        ServiceState.Value = state;
        Accounts.Value = accounts;
        Rewards.Value = rewards;
        MoneySupply.Value = supply;
    }
}