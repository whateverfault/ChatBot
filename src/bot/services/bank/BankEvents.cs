using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;
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

        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;
        
        client.OnMessageReceived += DepositWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();

        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;
        
        client.OnMessageReceived -= DepositWrapper;
    }
    
    private void DepositWrapper(object? sender, ChatMessage chatMessage) {
        var rewardId = chatMessage.RewardId;
        if (_bank == null || string.IsNullOrEmpty(rewardId)) return;

        if (!_bank.GetBalance(chatMessage.UserId, out var oldBalance)) {
            oldBalance = 0;
        }
        
        if (!_bank.Options.GetReward(rewardId, out var quantity)) return;
        if (!_bank.Deposit(chatMessage.UserId, quantity, gain: false)) return;

        var client = TwitchChatBot.Instance.GetClient();
        client?.SendMessage($"+{quantity} | {oldBalance} -> {oldBalance+quantity}", chatMessage.Id);
    }
}