using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.bot_lifecycle;

public class BotLifecycleService : Service {
    private readonly object _sync = new object();

    private bool _starting = false;
    private bool _stopping = false;
    
    public override BotLifecycleOptions Options { get; } = new BotLifecycleOptions();


    public async Task BotStart(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        lock (_sync) {
            if (_starting)
                return;
            
            _starting = true;
        }
        
        if (Options.ServiceState == State.Disabled
         || !streamStateNew.Online 
         || BotOnline()) {
            lock (_sync) {
                _starting = false;
            }
            return;
        }
        
        await TwitchChatBot.Instance.Start();
        
        lock (_sync) {
            _starting = false;
        }
    }
    
    public async Task BotStop(StreamState streamState, StreamData? streamData) {
        lock (_sync) {
            if (_stopping)
                return;
            
            _stopping = true;
        }
        
        if (Options.ServiceState == State.Disabled
         || streamState.Online 
         || !BotOnline()
         || streamState.OfflineTime < Options.GetDisconnectTimeout()) {
            lock (_sync) {
                _stopping = false;
            }
            return;
        }

        await TwitchChatBot.Instance.Stop();
        
        lock (_sync) {
            _stopping = false;
        }
    }

    public bool BotOnline() {
        return TwitchChatBot.Instance.Online;
    }
}