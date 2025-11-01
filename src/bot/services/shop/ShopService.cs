using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.shop;

public enum Lot {
    Ai = 0,
    Mute = 1,
    AntiMute = 2,
}

public class ShopService : Service {
    private static readonly BankService _bank = (BankService)ServiceManager.GetService(ServiceName.Bank);
    
    public override string Name => ServiceName.Shop;
    public override ShopOptions Options { get; } = new ShopOptions();

    public List<ShopLot> Lots => Options.Lots;


    public Result<ShopLot?, ErrorCode?> Buy(string userId, string lotName, long amount) {
        if (amount <= 0) {
            return new Result<ShopLot?, ErrorCode?>(null, ErrorCode.InvalidInput);
        }
        
        var lot = Get(lotName);
        if (lot == null) {
            return new Result<ShopLot?, ErrorCode?>(null, ErrorCode.NotFound);
        } if (lot.State == State.Disabled) {
            return new Result<ShopLot?, ErrorCode?>(lot, ErrorCode.ServiceDisabled);
        }
        
        var result = _bank.TakeOut(userId, lot.Cost * amount, gain: false);
        if (!result.Ok) {
            return new Result<ShopLot?, ErrorCode?>(lot, result.Error);
        }

        lot.AddBuyer(userId, amount);
        return new Result<ShopLot?, ErrorCode?>(lot, null);
    }
    
    public Result<ShopLot?, ErrorCode?> Use(string userId, string lotName) {
        var lot = Get(lotName);
        if (lot == null) {
            return new Result<ShopLot?, ErrorCode?>(null, ErrorCode.NotFound);
        } if (lot.State == State.Disabled) {
            return new Result<ShopLot?, ErrorCode?>(lot, ErrorCode.ServiceDisabled);
        } if (!lot.Use(userId)) {
            return new Result<ShopLot?, ErrorCode?>(lot, ErrorCode.NotEnough);
        }
        return new Result<ShopLot?, ErrorCode?>(lot, null);
    }

    public Result<ShopLot?, ErrorCode?> Give(string userId, string lotName, long amount = 1) {
        var lot = Get(lotName);
        if (lot == null) {
            return new Result<ShopLot?, ErrorCode?>(null, ErrorCode.NotFound);
        } if (lot.State == State.Disabled) {
            return new Result<ShopLot?, ErrorCode?>(lot, ErrorCode.ServiceDisabled);
        } 
        
        var depositResult = _bank.Deposit(userId, lot.Cost * amount, gain: false);
        if (!depositResult.Ok) return new Result<ShopLot?, ErrorCode?>(null, depositResult.Error);
        
        var buyResult = Buy(userId, lot.Name, amount);
        if (!buyResult.Ok) return new Result<ShopLot?, ErrorCode?>(null, buyResult.Error);
            
        return new Result<ShopLot?, ErrorCode?>(lot, null);
    }
    
    public bool Add(string name, long cost) {
        var lot = new ShopLot(name, cost);
        return Options.AddLot(lot);
    }
    
    public bool Remove(string name) {
        return Options.RemoveLot(name);
    }
    
    public ShopLot? Get(string name) {
        return Options.GetLot(name);
    }
    
    public ShopLot? Get(Lot lot) {
        return Options.GetLot(lot);
    }
}