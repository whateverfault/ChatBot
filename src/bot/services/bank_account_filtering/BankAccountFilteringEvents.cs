using ChatBot.bot.chat_bot;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.bank_account_filtering;

public class BankAccountFilteringEvents : ServiceEvents {
    private BankService? _bank;
    private BankAccountFilteringService? _accountFiltering;
    private StreamStateCheckerService? _streamStateChecker;
    
    public override bool Initialized { get; protected set; }


    public override void Init(Service service) {
        _bank = (BankService)Services.Get(ServiceId.Bank);
        _accountFiltering = (BankAccountFilteringService)service;
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        if (_streamStateChecker == null)
            return;
        
        _streamStateChecker.OnStreamStateUpdateAsync += UpdateLastOnline;
        _streamStateChecker.OnStreamStateChanged += StreamStateChanged;
        
        TwitchChatBot.Instance.OnRewardRedeemed += UpdateActivityWrapper;
        TwitchChatBot.Instance.OnMessageReceived += UpdateActivityWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        if (_streamStateChecker == null)
            return;
        
        _streamStateChecker.OnStreamStateUpdateAsync -= UpdateLastOnline;
        _streamStateChecker.OnStreamStateChanged -= StreamStateChanged;
        
        TwitchChatBot.Instance.OnRewardRedeemed -= UpdateActivityWrapper;
        TwitchChatBot.Instance.OnMessageReceived -= UpdateActivityWrapper;
    }
    
    private void UpdateActivityWrapper(object? sender, RewardRedemption redemption) {
        _bank?.UpdateActivity(redemption.UserId);
    }
    
    private void UpdateActivityWrapper(object? sender, ChatMessage message) {
        _bank?.UpdateActivity(message.UserId);
    }

    private void StreamStateChanged(StreamState streamState, StreamData? data) {
        if (streamState.Online 
         || _accountFiltering == null)
            return;

        _accountFiltering.Options.SetStreamFiltered(false);
    }
    
    private async Task UpdateLastOnline(StreamState streamState, StreamData? data) {
        if (!streamState.Online 
         || _accountFiltering == null 
         || _accountFiltering.Options.GetStreamFiltered() 
         || streamState.OnlineTime < _accountFiltering.Options.GetMinStreamLength()
         || streamState.OnlineTime - _accountFiltering.Options.GetLastOnline() < _accountFiltering.Options.GetStreamsTimeGapThreshold())
            return;

        _accountFiltering.Options.SetStreamFiltered(true);
        
        if (!_accountFiltering.Options.LastActiveThresholdInitialized())
            _accountFiltering.Options.SetLastOnlineThreshold(streamState.LastOnline);
        
        if (!_accountFiltering.Options.StreamsThresholdBreached()) {
            _accountFiltering.Options.IncrementPassedStreams();
            return;
        }

        await _accountFiltering.StartAccountFiltering();
        _accountFiltering.Options.SetLastOnlineThreshold(streamState.LastOnline);
        _accountFiltering.Options.ZeroPassedStreams();
    }
}