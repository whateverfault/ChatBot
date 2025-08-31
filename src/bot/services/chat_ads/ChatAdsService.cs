using ChatBot.api.twitch.helix.data.requests;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_ads.Data;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker.Data;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsService : Service {
    private static readonly ChatLogsService _chatLogs = (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
    
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
        var messageCount = _chatLogs.GetLogsCount();
        var ads = Options.GetChatAds();
        
        foreach (var ad in ads) {
            if (ad.GetState() == State.Disabled) continue;
            if (messageCount - ad.GetLastSentMessageCount() < ad.GetThreshold()) continue;
            if (now - streamState.StreamStart < ad.GetCooldown()
             || now - ad.GetLastTimeSent() < ad.GetCooldown()) continue;
            
            client.SendMessage(ad.GetOutput());
            ad.SetLastSentTime();
            ad.SetLastSentMessageCount(messageCount);
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