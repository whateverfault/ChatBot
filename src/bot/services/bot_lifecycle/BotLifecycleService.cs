using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.bot_lifecycle;

public class BotLifecycleService : Service {
    public override BotLifecycleOptions Options { get; } = new BotLifecycleOptions();


    public async Task BotStart(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled
         || !streamState.Online 
         || BotOnline()) {
            return;
        }
        
        await TwitchChatBot.Instance.Start();
    }
    
    public async Task BotStop(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled
         || streamState.Online 
         || !BotOnline()
         || streamState.OfflineTime < Options.GetDisconnectTimeout()) {
            return;
        }

        await TwitchChatBot.Instance.Stop();
    }

    public bool BotOnline() {
        return TwitchChatBot.Instance.Online;
    }
}