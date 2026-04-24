using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.casino;

public struct GambleResult {
    public double Win;
    public double Multiplier;
    public bool Result;
}

public struct DuelResult {
    public double Win;
    public string WinnerUserId;
    public string LooserUserId;
    public bool Result;

    public DuelResult(GambleResult gambleResult) {
        Win = gambleResult.Win;
        Result = gambleResult.Result;
        
        WinnerUserId = string.Empty;
        LooserUserId = string.Empty;
    }
}

public struct EmoteGambleResult {
    public double Win;
    public double Multiplier;
    public GambleEmote[] Emotes;
}

public class CasinoService : Service {
    private static BankService Bank => (BankService)Services.Get(ServiceId.Bank);
    
    public override CasinoOptions Options { get; } = new CasinoOptions();
    
    
    public Result<GambleResult?, ErrorCode?> Gamble(string userId, double quantity) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<GambleResult?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }
        
        var result = new GambleResult { Result = false, Win = 0, };
        
        if (!Bank.GetBalance(userId, out var balance)){
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.AccountNotFound);
        }
        
        if (!Bank.HasEnoughPoints(userId, quantity)) {
            return new Result<GambleResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        }
        
        result = CalculateGambleResult(quantity, balance);

        if (result.Result) {
            var depositResult = Bank.Deposit(userId, result.Win, gain: true);
            if (!depositResult.Ok) {
                return new Result<GambleResult?, ErrorCode?>(result, depositResult.Error);
            }
        }
        else {
            var takeOutResult = Bank.TakeOut(userId, quantity, gain: true);
            if (!takeOutResult.Ok) {
                return new Result<GambleResult?, ErrorCode?>(result, takeOutResult.Error);
            }
        }
        
        return new Result<GambleResult?, ErrorCode?>(result, null);
    }

    public Result<EmoteGambleResult?, ErrorCode?> GambleWithEmotes(string userId, double quantity) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<EmoteGambleResult?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }

        if (Options.Emotes.Count <= 0) {
            return new Result<EmoteGambleResult?, ErrorCode?>(null, ErrorCode.Empty);
        }
        
        var result = new EmoteGambleResult { Win = 0, };

        if (!Bank.HasEnoughPoints(userId, quantity)) {
            return new Result<EmoteGambleResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        }
        
        var emotes = new GambleEmote[Options.EmoteSlots];
        var sum = Options.Emotes.Sum(x => x.Part);
        
        for (var i = 0; i < Options.EmoteSlots; ++i) {
            var choice = Options.Emotes[0];
            var random = Random.Shared.NextDouble() * 100;
            var offset = 0.0;

            foreach (var emote in Options.Emotes) {
                if (offset > random) {
                    break;
                }

                choice = emote;
                offset += emote.Part/sum * 100;
            }

            emotes[i] = choice;
        }

        result.Emotes = emotes;

        var checkedEmotes = new Dictionary<string, bool>();
        foreach (var emote in emotes) {
            if (!checkedEmotes.TryAdd(emote.Name, true)) {
                continue;
            }

            var count = emotes.Count(x => x.Name == emote.Name);
            if (count < emote.Combo) {
                result.Multiplier += emote.BonusCoefficient * count;
                continue;
            }

            var bonusCount = count - emote.Combo;
            result.Multiplier += emote.ComboCoefficient;
            result.Multiplier += emote.BonusCoefficient * bonusCount;
        }
        
        result.Win = quantity * result.Multiplier - quantity;
        
        if (result.Win >= 0) {
            var depositResult = Bank.Deposit(userId, Math.Abs(result.Win), gain: true);
            if (!depositResult.Ok) {
                return new Result<EmoteGambleResult?, ErrorCode?>(result, depositResult.Error);
            }
        }
        else {
            var takeOutResult = Bank.TakeOut(userId, Math.Abs(result.Win), gain: true);
            if (!takeOutResult.Ok) {
                return new Result<EmoteGambleResult?, ErrorCode?>(result, takeOutResult.Error);
            }
        }
        
        return new Result<EmoteGambleResult?, ErrorCode?>(result, null);
    }
    
    private (double chance, double multiplier) CalculateChances(double betAmount, double userBalance, double moneySupply) {
        if (moneySupply <= 0) moneySupply = 1;
        var riskFactor = Math.Clamp(betAmount / (userBalance * 0.5f), 0.1f, 2f);
        var multiplier = Options.BaseCoefficient + Options.RandomValue * Options.AdditionalCoefficient * riskFactor;
        var chance = 1f / (multiplier*1.35f);
        var wealthPenalty = 1f - Math.Min(0.3f, (userBalance / moneySupply) / 2);
        
        chance *= wealthPenalty;
        chance = Math.Max(chance, 0.1f);
    
        return (chance, multiplier);
    }

    private GambleResult CalculateGambleResult(double quantity, double balance) {
        var result = new GambleResult { Result = false, Win = 0, Multiplier = 0, };
        
        var (chances, multiplier) = CalculateChances(quantity, balance, Bank.MoneySupply);
        result.Multiplier = multiplier * 0.2;
        
        var random = Random.Shared.NextDouble();
        if (random <= chances) {
            result.Multiplier = multiplier;
            result.Result = true;
        } 
        
        result.Win = Math.Ceiling(quantity * multiplier);
        result.Multiplier = multiplier;
        
        Options.NewRandomValue();
        return result;
    }
    
    public Result<bool?, ErrorCode?> CreateDuel(string subjectId, string objectId, double quantity) {
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
                   new Result<bool?, ErrorCode?>(false, ErrorCode.AlreadyExists);
    }
    
    public Result<DuelResult?, ErrorCode?> AcceptDuel(string objectId, string? subjectId = null) {
        if (Options.ServiceState == State.Disabled) {
            return new Result<DuelResult?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }

        var result = new DuelResult { Result = false, };
        
        Duel? duel;
        if ((duel = Options.GetDuel(objectId, subjectId)) == null) {
            return new Result<DuelResult?, ErrorCode?>(result, ErrorCode.NoAvailable);
        }
        
        if (!Bank.GetBalance(duel.Subject, out var subjectBalance) || subjectBalance < duel.Quantity) {
            return new Result<DuelResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        } 
        if (!Bank.GetBalance(duel.Object, out var objectBalance) || objectBalance < duel.Quantity) {
            return new Result<DuelResult?, ErrorCode?>(result, ErrorCode.TooFewPoints);
        }
        
        var takeOutResult = Bank.TakeOut(duel.Subject, duel.Quantity, gain: true);
        if (!takeOutResult.Ok) {
            return new Result<DuelResult?, ErrorCode?>(result, takeOutResult.Error);
        } 
        
        takeOutResult = Bank.TakeOut(duel.Object, duel.Quantity, gain: true);
        if (!takeOutResult.Ok) {
            Bank.Deposit(duel.Subject, duel.Quantity, gain: true);
            return new Result<DuelResult?, ErrorCode?>(result, takeOutResult.Error);
        }
        
        var balance = (subjectBalance + objectBalance) / 2;
        result = new DuelResult(CalculateGambleResult(duel.Quantity, balance));
        
        var random = Random.Shared.Next(0, 2);
        result.WinnerUserId = random == 0? 
                                  duel.Subject :
                                  duel.Object;
        
        result.LooserUserId = random == 0? 
                                  duel.Object :
                                  duel.Subject; 

        result.Win = duel.Quantity;
        result.Result = true;
        
        Bank.Deposit(result.WinnerUserId, result.Win*2, gain: true);
        Options.RemoveDuel(objectId, subjectId);
        
        return new Result<DuelResult?, ErrorCode?>(result, null);
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