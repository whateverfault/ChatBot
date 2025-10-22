using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.shop;

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
    
    public ShopLot? Get(int id) {
        return Options.GetLot(id);
    }
}