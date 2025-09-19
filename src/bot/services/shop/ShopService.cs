using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.shop;

public class ShopService : Service {
    public override string Name => ServiceName.Shop;
    public override ShopOptions Options { get; } = new ShopOptions();

    public ShopLot[] Lots => Options.Lots;
    
    public ShopLot? GetLot(string name) => Options.Lots.FirstOrDefault(lot => lot.Name.Equals(name));
}