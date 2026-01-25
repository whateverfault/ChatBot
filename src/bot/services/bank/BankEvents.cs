using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.bank;

public class BankEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }

    private BankService? _bank;


    public override void Init(Service service) {
        _bank = (BankService)service;
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        TwitchChatBot.Instance.OnRewardRedeemed += DepositWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnRewardRedeemed -= DepositWrapper;
    }
    
    private async void DepositWrapper(object? sender, RewardRedemption redemption) {
        try {
            var client = TwitchChatBot.Instance.GetClient();
            if (client == null) return;
        
            var rewardId = redemption.Reward.Id;
            if (_bank == null || string.IsNullOrEmpty(rewardId)) return;

            if (!_bank.GetBalance(redemption.UserId, out var oldBalance)) {
                oldBalance = 0;
            }
        
            if (!_bank.Options.GetReward(rewardId, out var quantity)) return;

            var result = _bank.Deposit(redemption.UserId, quantity, gain: false);
            if (!result.Ok) {
                await ErrorHandler.SendError(result.Error, client, redemption.UserName);
                return;
            }
        
            await client.SendMessage($"+{quantity} | {oldBalance} -> {oldBalance+quantity} @{redemption.UserName} ");
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, e.Message);
        }
    }
}