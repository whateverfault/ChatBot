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
         || distUserId.Equals(client.Credentials.Broadcaster.UserId))
            return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);


        var takeResult = TakeOut(srcUserId, quantity, gain: false);
        if (!takeResult.Ok) {
            return new Result<bool, ErrorCode?>(false, takeResult.Error);
        }

        var giveResult = Options.Deposit(distUserId, quantity, gain: false);
        return new Result<bool, ErrorCode?>(takeResult.Value && giveResult, null);
    }

    public Result<bool, ErrorCode?> Give(Account dist, Account src, long quantity) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);

        var takeResult = TakeOut(src, quantity, gain: false);
        if (!takeResult.Ok) {
            return new Result<bool, ErrorCode?>(false, takeResult.Error);
        }

        var giveResult = Options.Deposit(dist, quantity, gain: false);
        return new Result<bool, ErrorCode?>(takeResult.Value && giveResult, null);
    }
    
    public Result<bool, ErrorCode?> TakeOut(string userId, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);

        if (userId.Equals(client.Credentials.Broadcaster.UserId)) return new Result<bool, ErrorCode?>(true, null);

        var result = Options.TakeOut(userId, quantity, gain);
        return result
                   ? new Result<bool, ErrorCode?>(true, null)
                   : new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
    }

    public Result<bool, ErrorCode?> TakeOut(Account? account, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);

        var result = Options.TakeOut(account, quantity, gain);
        return result
                   ? new Result<bool, ErrorCode?>(true, null)
                   : new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
    }
    
    public Result<bool, ErrorCode?> Deposit(string userId, long quantity, bool gain = true) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);

        if (userId.Equals(client.Credentials.Broadcaster.UserId))
            return new Result<bool, ErrorCode?>(false, ErrorCode.UserNotFound);
        return new Result<bool, ErrorCode?>(Options.Deposit(userId, quantity, gain), null);
    }

    public bool GetBalance(string userId, out long balance) {
        balance = long.MaxValue;

        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return false;

        return userId.Equals(client.Credentials.Broadcaster.UserId) || Options.GetBalance(userId, out balance);
    }

    public Result<Dictionary<string, long>?, ErrorCode?> Giveaway(string userId, long money) {
        if (!GetAccount(userId, out var account) || account == null)
            return new Result<Dictionary<string, long>?, ErrorCode?>(null, ErrorCode.AccountNotFound);

        return Giveaway(account, money);
    }

    public Result<Dictionary<string, long>?, ErrorCode?> Giveaway(Account account, long money) {
        var accounts = Options.GetAccounts();

        if (account.Money < money) 
            return new Result<Dictionary<string, long>?, ErrorCode?>(null, ErrorCode.TooFewPoints);
        
        var random = Random.Shared;
        var amount = accounts.Count switch {
                         >= 1 => (int)random.NextInt64(1, Math.Min(money, accounts.Count)%int.MaxValue),
                         _   => 0,
                     };

        if (amount < 1) 
            return new Result<Dictionary<string, long>?, ErrorCode?>(null, ErrorCode.TooFewAccounts);
        
        if (amount > 10) amount = 10;
        
        var quantityPerEach = money / amount;
        if (quantityPerEach <= 0) {
            return new Result<Dictionary<string, long>?, ErrorCode?>(null, ErrorCode.TooFewPoints);
        }

        var map = new Dictionary<string, long>();
        var left = money;
        var retries = 0;
        
        for (var i = 0; i < amount; ++i) {
            var index = random.Next(0, accounts.Count);
            var (_, receiver) = accounts.ElementAt(index);

            if (receiver.UserId.Equals(account.UserId)) {
                if (index - 1 < 0 && index + 1 < accounts.Count) ++index;
                else if (index - 1 >= 0) --index;
                else return new Result<Dictionary<string, long>?, ErrorCode?>(null, ErrorCode.TooFewAccounts);
                
                (_, receiver) = accounts.ElementAt(index);
            }
            
            var result = Give(receiver, account, quantityPerEach);
            if (!result.Ok) {
                if (retries++ >= 3) break;
                --i;
                continue;
            }

            if (!map.TryAdd(receiver.UserId, quantityPerEach)) {
                map[receiver.UserId] += quantityPerEach;
            }

            left -= quantityPerEach;
            
            var tempAmount = amount - (i + 1);
            if (tempAmount < 1) tempAmount = 1;
            
            quantityPerEach = left / tempAmount;
        }

        return new Result<Dictionary<string, long>?, ErrorCode?>(map, null);
    }

    public bool DeleteAccount(string userId) {
        return Options.DeleteAccount(userId);
    }
    
    public bool GetAccount(string userId, out Account? account) { 
        return Options.GetAccount(userId, out account);
    }

    public void UpdateActivity(string userId) {
        Options.UpdateActivity(userId);
    }
    
    public long GetMoneySupply() {
        return MoneySupply;
    }
}