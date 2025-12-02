using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.shop.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<List<ShopLot>> Lots = new SafeField<List<ShopLot>>(
                                                                                 [
                                                                                     new ShopLot("Ai", 50, isDefault: true, state: State.Disabled),
                                                                                     new ShopLot("Mute", 10000, isDefault: true, state: State.Disabled),
                                                                                     new ShopLot("AntiMute", 7500, isDefault: true, state: State.Disabled),
                                                                                 ]
                                                                                );
    
    
    public SaveDataDto() { }
    
    public SaveDataDto(
        State state,
        List<ShopLot> shop) {
        ServiceState.Value = state;
        Lots.Value = shop;
    }
}