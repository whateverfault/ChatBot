using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.helix;
using TwitchAPI.shared;

namespace ChatBot.bot.services.shop.data;

public static class DefaultLots {
    private static readonly ShopService   _shop = (ShopService)ServiceManager.GetService(ServiceName.Shop);
    private static readonly BankService   _bank = (BankService)ServiceManager.GetService(ServiceName.Bank);
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static readonly AiService     _ai   = (AiService)ServiceManager.GetService(ServiceName.Ai);
    
    
    public static async Task<Result<string?, ErrorCode?>> Ai(string userId, string prompt) {
        if (_ai.Options.ServiceState == State.Disabled) {
            return new Result<string?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }
        
        var lot = _shop.Get(ServiceName.Ai);
        if (lot == null) {
            return new Result<string?, ErrorCode?>(null, ErrorCode.NotFound);
        } if (lot.State == State.Disabled) {
            return new Result<string?, ErrorCode?>(null, ErrorCode.ServiceDisabled);
        }
        
        var result = _shop.Use(userId, lot.Name);
        if (result.Ok) return await _ai.GetResponse(prompt);

        var takeOutResult = _bank.TakeOut(userId, lot.Cost, gain: false);
        if (!takeOutResult.Ok) {
            return new Result<string?, ErrorCode?>(null, ErrorCode.TooFewPoints);
        }

        return await _ai.GetResponse(prompt);
    }
    
    public static async Task<Result<bool, ErrorCode?>> Mute(string userId) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null) return new Result<bool, ErrorCode?>(false, ErrorCode.NotInitialized);
        
        var lot = _shop.Get(1);
        if (lot == null) {
            return new Result<bool, ErrorCode?>(false, ErrorCode.NotFound);
        } if (lot.State == State.Disabled) {
            return new Result<bool, ErrorCode?>(false, ErrorCode.ServiceDisabled);
        }

        var result = _shop.Use(userId, lot.Name);
        if (!result.Ok) {
            var takeOutResult = _bank.TakeOut(userId, lot.Cost, gain: false);
            if (!takeOutResult.Ok) {
                return new Result<bool, ErrorCode?>(false, ErrorCode.TooFewPoints);
            }
        }
        
        await Helix.TimeoutUserHelix(userId, "За фантики", TimeSpan.FromMinutes(5), client.Credentials, (_, message) => {
                                         _logger.Log(LogLevel.Error, message);
                                     });
        return new Result<bool, ErrorCode?>(true, null);
    }
}