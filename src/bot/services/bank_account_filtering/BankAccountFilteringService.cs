using System.Text;
using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.bank.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.bot.services.bank_account_filtering;

public class BankAccountFilteringService : Service {
    private static BankService Bank => (BankService)Services.Get(ServiceId.Bank);

    public override BankAccountFilteringOptions Options { get; } = new BankAccountFilteringOptions();
    
    
    public async Task StartAccountFiltering() {
        if (Options.ServiceState != State.Enabled)
            return;
        
        var bot = TwitchChatBot.Instance;
        var client = bot.GetClient();
        if (client?.Credentials == null)
            return;
        
        var accounts = Bank.Options.GetAccounts();
        var accountsToDelete = new List<Account>();
        var accountsToGivePoints = new Dictionary<string, long>();
        var money = 0L;

        foreach (var (_, account) in accounts) {
            if (account.LastActive >= Options.GetLastActiveThreshold())
                continue;
            
            if (account.Money <= 0) {
                Bank.DeleteAccount(account.UserId);
                continue;
            }

            accountsToDelete.Add(account);
            Bank.DeleteAccount(account.UserId);
        }

        accounts = Bank.Options.GetAccounts();
        if (accounts.Count <= 0)
            return;
        
        foreach (var account in accountsToDelete) {
            var result = Bank.Giveaway(account, account.Money);
            if (!result.Ok) 
                continue;
            
            var tempMap = result.Value;
            if (tempMap == null) 
                continue;
            
            accountsToGivePoints = accountsToGivePoints.Concat(tempMap)
                     .GroupBy(kvp => kvp.Key)
                     .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

            money += tempMap.Sum(x => x.Value);
        }

        if (money <= 0) {
            return;
        }
        
        var sb = new StringBuilder();
        for (var i = 0; i < accountsToGivePoints.Count; ++i) {
            var (receiver, points) = accountsToGivePoints.ElementAt(i);
            var username = await bot.Api.GetUserName(receiver, client.Credentials, true, (_, msg) => {
                                                         ErrorHandler.LogMessage(LogLevel.Error, msg);
                                                     });
            if (username == null) continue;
            
            sb.Append($"{username} - {points} {(i >= accountsToGivePoints.Count-1? string.Empty : "/ ")}");
        }
        
        await client.SendMessage($"Удалено {accountsToDelete.Count} {Declensioner.Accounts(accountsToDelete.Count)} и роздано {money} {Declensioner.Points(money)}: {sb}");
    }
}