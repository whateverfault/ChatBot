using ChatBot.bot.chat_bot;
using ChatBot.bot.services.bank.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.bank;

public class BankService : Service {
    public override string Name => ServiceName.Bank;
    public override BankOptions Options { get; } = new BankOptions();

    public long MoneySupply => Options.MoneySupply;
    

    public Result<bool, ErrorCode?> Give(string distUserId, string srcUserId, long quantity) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        if (distUserId.Equals(client.Credentials.UserId)
         || distUserId.Equals(client.Credentials.ChannelId)) return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        
        
        var takeResult = Options.TakeOut(srcUserId, quantity, gain: false);
        if (!takeResult) return new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
        
        var giveResult = Options.Deposit(distUserId, quantity, gain: false);
        return new Result<bool, ErrorCode?>(takeResult && giveResult, null);
    }
    
    public bool TakeOut(string userId, long quantity, bool gain = true) => Options.TakeOut(userId, quantity, gain);

    public Result<bool, ErrorCode?> Deposit(string userId, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        if (userId.Equals(client.Credentials.UserId)
         || userId.Equals(client.Credentials.ChannelId)) return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        
        return new Result<bool, ErrorCode?>(Options.Deposit(userId, quantity, gain), null);
    }

    public bool GetBalance(string userId, out long balance) => Options.GetBalance(userId, out balance);
    public bool GetAccount(string userId, out Account? account) => Options.GetAccount(userId, out account);
    
    public long GetMoneySupply() {
        return MoneySupply;
    }
}