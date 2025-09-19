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
        if (distUserId.Equals(client.Credentials.UserId)) return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        
        
        var takeResult = Options.TakeOut(srcUserId, quantity, gain: false);
        if (!takeResult) return new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
        
        var giveResult = Options.Deposit(distUserId, quantity, gain: false);
        return new Result<bool, ErrorCode?>(takeResult && giveResult, null);
    }
    
    public bool TakeOut(string userId, long quantity, bool gain = true) => Options.TakeOut(userId, quantity, gain);
    public bool Deposit(string userId, long quantity, bool gain = true) => Options.Deposit(userId, quantity, gain);
    
    public bool GetBalance(string userId, out long balance) => Options.GetBalance(userId, out balance);
    public bool GetAccount(string userId, out Account? account) => Options.GetAccount(userId, out account);
}