using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

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
        if (moneySupply <= 0) moneySupply = 1;
        var riskFactor = Math.Clamp(betAmount / (moneySupply * 0.1f), 0.1f, 2f);
        var multiplier = Options.BaseMultiplier + Options.RandomValue * Options.AdditionalMultiplier * riskFactor;
        var chance = 1f / (multiplier*1.5f);
        var wealthPenalty = 1f - Math.Min(0.3f, userBalance / (moneySupply * 2f));
        
        chance *= wealthPenalty;
        chance = Math.Max(chance, 0.05f);
    
        return (chance, multiplier);
    }
    
    public Result<GambleResult?, ErrorCode?> Gamble(string userId, long quantity) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<GambleResult?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }
        
        var result = new GambleResult { Ok = true, Result = false, };
        if (!Bank.GetBalance(userId, out var balance)
         || !Bank.TakeOut(userId, quantity)) {
            result.Ok = false;
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
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
        return new Result<GambleResult?, ErrorCode?>(result, null);
    }
}