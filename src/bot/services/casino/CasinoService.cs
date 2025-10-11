using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.casino;

public struct GambleResult {
    public bool Ok;
    public long Win;
    public string UserId;
    public double Multiplier;
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
        
        var result = new GambleResult { Ok = true, Result = false, Win = -quantity, UserId = userId, };
        var takeOutResult = Bank.TakeOut(userId, quantity, gain: true);
        
        if (!Bank.GetBalance(userId, out var balance)){
            result.Ok = false;
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.AccountNotFound);
        } if (!takeOutResult.Ok) {
            result.Ok = false;
            return new Result<GambleResult?, ErrorCode?>(result, takeOutResult.Error);
        }

        result = CalculateGambleResult(quantity, balance);
        result.UserId = userId;
        
        Bank.Deposit(userId, result.Win, gain: true);
        return new Result<GambleResult?, ErrorCode?>(result, null);
    }

    private GambleResult CalculateGambleResult(long quantity, long balance) {
        var result = new GambleResult { Ok = true, Result = false, Win = -quantity, };
        
        var (chances, multiplier) = CalculateChances(quantity, balance, Bank.MoneySupply);
        var potentialWin = (long)(quantity * multiplier);
        
        var random = Random.Shared.NextSingle();
        if (random <= chances) {
            result.Result = true;
            result.Win = potentialWin;
        } 
        
        result.Multiplier = multiplier;
        Options.NewRandomValue();
        return result;
    }
    
    public Result<bool?, ErrorCode?> CreateDuel(string subjectId, string objectId, long quantity) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<bool?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }

        if (subjectId.Equals(objectId)) {
            return new Result<bool?, ErrorCode?>(false, ErrorCode.AccountNotFound);
        }
        
        if (!Bank.GetBalance(subjectId, out var balance)
         || balance < quantity) {
            return new Result<bool?, ErrorCode?>(false, ErrorCode.TooFewPoints);
        }

        var result = Options.AddDuel(subjectId, objectId, quantity);
        return result?
                   new Result<bool?, ErrorCode?>(true, null) : 
                   new Result<bool?, ErrorCode?>(false, ErrorCode.AlreadyContains);
    }
    
    public Result<GambleResult?, ErrorCode?> AcceptDuel(string objectId, string? subjectId = null) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<GambleResult?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }

        var result = new GambleResult { Ok = true, Result = false, };
        
        Duel? duel;
        if ((duel = Options.GetDuel(objectId, subjectId)) == null) {
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.NoAvailable);
        }
        
        if (!Bank.GetBalance(duel.Subject, out var subjectBalance)
         || subjectBalance < duel.Quantity) {
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        } if (!Bank.GetBalance(duel.Object, out var objectBalance)
           || objectBalance < duel.Quantity) {
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        }
        
        var takeOutResult = Bank.TakeOut(duel.Subject, duel.Quantity, gain: true);
        if (!takeOutResult.Ok) {
            return new Result<GambleResult?, ErrorCode?>(result, takeOutResult.Error);
        } 
        
        takeOutResult = Bank.TakeOut(duel.Object, duel.Quantity, gain: true);
        if (!takeOutResult.Ok) {
            return new Result<GambleResult?, ErrorCode?>(result, takeOutResult.Error);
        }
        
        var balance = (subjectBalance + objectBalance) / 2;
        result = CalculateGambleResult(duel.Quantity, balance);

        var random = Random.Shared.Next(0, 2);
        var winner = random == 0? 
                         duel.Subject :
                         duel.Object;

        result.Win = duel.Quantity;
        result.Result = true;
        result.UserId = winner;
        
        Bank.Deposit(winner, result.Win*2, gain: true);
        Options.RemoveDuel(objectId, subjectId);
        
        return new Result<GambleResult?, ErrorCode?>(result, null);
    }

    public Result<bool, ErrorCode?> DeclineDuel(string objectId, string? subjectId = null) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<bool, ErrorCode?>(false, ErrorCode.ServiceDisabled);
        }

        var result = Options.RemoveDuel(objectId, subjectId);
        return result? 
                   new Result<bool, ErrorCode?>(true, null) :
                   new Result<bool, ErrorCode?>(false, ErrorCode.NotFound);
    }
    
    public Result<bool, ErrorCode?> RemoveDuels(string subjectId) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<bool, ErrorCode?>(false, ErrorCode.ServiceDisabled);
        }

        Options.RemoveDuels(subjectId);
        return new Result<bool, ErrorCode?>(true, null);
    }
    
    public Result<List<Duel>?, ErrorCode?> ListDuels(string objectId) {
        var duels= Options.GetDuels(objectId);
        return duels.Count <= 0 ? 
                   new Result<List<Duel>?, ErrorCode?>(null, ErrorCode.Empty) : 
                   new Result<List<Duel>?, ErrorCode?>(duels, null);
    }
}