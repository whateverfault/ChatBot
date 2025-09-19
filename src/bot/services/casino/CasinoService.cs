using ChatBot.bot.services.bank;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.casino;

public struct GambleResult {
    public bool Ok;
    public float Multiplier;
    public bool Result;
}

public class CasinoService : Service {
    private static BankService Bank => (BankService)ServiceManager.GetService(ServiceName.Bank);
    
    public override string Name => ServiceName.Casino;
    public override CasinoOptions Options { get; } = new CasinoOptions();


    private (float chance, float multiplier) CalculateChances(long betAmount, long userBalance, long moneySupply) {
        var riskFactor = Math.Clamp(betAmount / (moneySupply * 0.1f), 0.1f, 2f);
        var multiplier = 1.5f + Options.RandomValue * 1.3f * riskFactor;
        var chance = 1f / (multiplier*1.5f);
        var wealthPenalty = 1f - Math.Min(0.3f, userBalance / (moneySupply * 2f));
        
        chance *= wealthPenalty;
        chance = Math.Max(chance, 0.05f);
    
        return (chance, multiplier);
    }

    public void Deposit(string userId, long quantity) {
        Bank.Deposit(userId, quantity, gain: false);
    }
    
    public GambleResult Gamble(string userId, long quantity) {
        var result = new GambleResult { Ok = true, Result = false, };
        if (!Bank.GetBalance(userId, out var balance)
         || !Bank.TakeOut(userId, quantity)) {
            result.Ok = false;
            return result;
        }
        
        var (chances, multiplier) = CalculateChances(quantity, balance, Bank.MoneySupply);
        var potentialWin = (long)(quantity * multiplier);
        
        var random = Random.Shared.NextSingle();
        if (random <= chances) {
            Bank.Deposit(userId, potentialWin);
            result.Result = true;
        } 
        
        result.Multiplier = multiplier;
        Options.NewRandomValue();
        return result;
    }

    public long GetMoneySupply() {
        return Bank.MoneySupply;
    }
}