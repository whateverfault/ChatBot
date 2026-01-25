using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_ads.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsService : Service {
    public override ChatAdsOptions Options { get; } = new ChatAdsOptions();

    public event EventHandler<ChatAd>? OnChatAdAdded;
    public event EventHandler<int>? OnChatAdRemoved;
    

    public void SendChatAds(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled) return;
        if (!streamState.Online || streamState.OnlineTime <= 0) return;
        
        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ads = Options.GetChatAds();
        
        foreach (var ad in ads) {
            if (ad.GetState() == State.Disabled) continue;
            if (ad.GetCounter() < ad.GetThreshold()) continue;
            if (now - streamState.StreamStart < ad.GetCooldown()
             || now - ad.GetLastTimeSent() < ad.GetCooldown()) continue;
            
            client.SendMessage(ad.GetOutput());
            ad.SetLastSentTime();
            ad.ZeroCounter();
        }
    }
    
    public void AddChatAd(ChatAd chatAd) {
        Options.AddChatAd(chatAd);
        OnChatAdAdded?.Invoke(this, chatAd);
    }

    public bool RemoveChatAd(int index) {
        var result = Options.RemoveChatAd(index);
        if (result) {
            OnChatAdRemoved?.Invoke(this, index);
        } 
            
        return result;
    }
    
    public List<ChatAd> GetChatAds() {
        return Options.GetChatAds();
    }
}