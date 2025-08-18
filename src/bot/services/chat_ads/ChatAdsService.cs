using ChatBot.api.twitch.shared.requests.data;
using ChatBot.bot.services.chat_ads.Data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsService : Service {
    public override string Name => ServiceName.ChatAds;
    public override ChatAdsOptions Options { get; } = new ChatAdsOptions();

    public event EventHandler<ChatAd>? OnChatAdAdded;
    public event EventHandler<int>? OnChatAdRemoved;
    

    public void HandleChatAdsSending(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled) return;
        if (!streamState.WasOnline || streamState.OnlineTime <= 0) return;
        
        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ads = Options.GetChatAds();
        
        foreach (var ad in ads) {
            if (ad.GetState() == State.Disabled) continue;
            if (now - streamState.LastOnline < ad.GetCooldown()
             || now - ad.GetLastTimeSent() < ad.GetCooldown()) continue;
            
            client.SendMessage(ad.GetOutput());
            ad.SetLastSentTime();
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