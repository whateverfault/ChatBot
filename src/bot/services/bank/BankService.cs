using ChatBot.bot.chat_bot;
using ChatBot.bot.services.bank.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.bank;

public class BankService : Service {
    public override BankOptions Options { get; } = new BankOptions();

    public long MoneySupply => Options.MoneySupply;
    

    public Result<bool, ErrorCode?> Give(string distUserId, string srcUserId, long quantity) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        if (distUserId.Equals(srcUserId)
         || distUserId.Equals(client.Credentials.ChannelId)) return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        
        
        var takeResult = TakeOut(srcUserId, quantity, gain: false);
        if (!takeResult.Ok) {
            return new Result<bool, ErrorCode?>(false, takeResult.Error);
        }
        
        var giveResult = Options.Deposit(distUserId, quantity, gain: false);
        return new Result<bool, ErrorCode?>(takeResult.Value && giveResult, null);
    }

    public Result<bool, ErrorCode?> TakeOut(string userId, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        if (userId.Equals(client.Credentials.ChannelId)) return new Result<bool, ErrorCode?>(true,null);
        
        var result = Options.TakeOut(userId, quantity, gain);
        return result? 
                   new Result<bool, ErrorCode?>(true, null) :
                   new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
    }

    public Result<bool, ErrorCode?> Deposit(string userId, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        if (userId.Equals(client.Credentials.ChannelId)) return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        return new Result<bool, ErrorCode?>(Options.Deposit(userId, quantity, gain), null);
    }

    public bool GetBalance(string userId, out long balance) {
        balance = long.MaxValue;
        
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return false;
        
        return userId.Equals(client.Credentials.ChannelId) || Options.GetBalance(userId, out balance);
    }

    public bool GetAccount(string userId, out Account? account) => Options.GetAccount(userId, out account);
    
    public long GetMoneySupply() {
        return MoneySupply;
    }
}