using ChatBot.bot;
using ChatBot.services.interfaces;
using ChatBot.services.Static;
using ChatBot.services.stream_state_checker.Data;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils.Twitch.Helix.Data;

namespace ChatBot.services.chat_ads;

public class ChatAdsService : Service {
    public override string Name => ServiceName.ChatAds;
    public override ChatAdsOptions Options { get; } = new ChatAdsOptions();


    public void SendAdsIfNeeded(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled) return;
        if (!streamState.WasOnline || streamState.OnlineTime <= 0) return;

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ads = Options.GetChatAds();
        foreach (var ad in ads) {
            if (ad.GetState() == State.Disabled) continue;
            if (now - ad.GetLastTimeSent() < ad.GetCooldown()) continue;

            var bot = TwitchChatBot.Instance;
            if (bot.TryGetClient(out var client) != ErrorCode.None) return;
            
            if (client == null) return;
            client.SendMessage(bot.Options.Channel, ad.GetOutput());
            ad.SetLastSentTime();
        }
    }
}