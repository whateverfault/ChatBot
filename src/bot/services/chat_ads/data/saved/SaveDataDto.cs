using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.chat_ads.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled); 
        
    public readonly SafeField<List<ChatAd>> ChatAds = new SafeField<List<ChatAd>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        List<ChatAd> chatAds) {
        ServiceState.Value = serviceState;
        ChatAds.Value = chatAds;
    }
}