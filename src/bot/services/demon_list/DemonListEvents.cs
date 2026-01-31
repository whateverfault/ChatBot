using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.demon_list;

public class DemonListEvents : ServiceEvents {
    private StreamStateCheckerService? _streamStateChecker;
    private DemonListService? _demonList;
    
    public override bool Initialized { get; protected set; }


    public override void Init(Service service) {
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        _demonList = (DemonListService)service;
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        if (_streamStateChecker == null) return;
        _streamStateChecker.OnStreamStateChanged += ResetCacheWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();

        if (_streamStateChecker == null) return;
        _streamStateChecker.OnStreamStateChanged -= ResetCacheWrapper;
    }
    
    private void ResetCacheWrapper(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        if (!streamStateNew.Online) return;
        
        _demonList?.ResetCache();
    }
}