using ChatBot.bot;
using ChatBot.services.interfaces;
using ChatBot.services.logger;
using ChatBot.services.Static;
using ChatBot.services.stream_state_checker.Data;
using ChatBot.utils.Twitch.Helix;
using ChatBot.utils.Twitch.Helix.Data;

namespace ChatBot.services.stream_state_checker;

public class StreamStateCheckerService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.StreamStateChecker;
    public override StreamStateCheckerOptions Options { get; } = new StreamStateCheckerOptions();

    public delegate void StreamStateHandler(StreamState streamState, StreamData? streamData);
    public delegate Task StreamStateHandlerAsync(StreamState streamState, StreamData? streamData);
    
    public event StreamStateHandler? OnStreamStateUpdate;
    public event StreamStateHandlerAsync? OnStreamStateUpdateAsync;
    
    public event StreamStateHandler? OnStreamStateChanged;
    public event StreamStateHandlerAsync? OnStreamStateChangedAsync;

    
    public async Task CheckState() {
        try {
            var bot = TwitchChatBot.Instance;
            var lastCheckedWasOnline = Options.StreamState.WasOnline;
            
            if (bot.Options.Channel == null) {
                return;
            }
            
            var streamResponse = await HelixUtils.GetStreams(bot.Options, bot.Options.Channel);
            if (streamResponse == null || streamResponse.Data.Count == 0) {
                if (lastCheckedWasOnline) {
                    OnStreamStateChangedAsync?.Invoke(Options.StreamState, null);
                    OnStreamStateChanged?.Invoke(Options.StreamState, null);
                }
                
                Options.AddOfflineTime();
                OnStreamStateUpdateAsync?.Invoke(Options.StreamState, null);
                OnStreamStateUpdate?.Invoke(Options.StreamState, null);
                return;
            }
            
            var streamData = streamResponse.Data[0];
            if (!lastCheckedWasOnline) {
                OnStreamStateChangedAsync?.Invoke(Options.StreamState, streamData);
                OnStreamStateChanged?.Invoke(Options.StreamState, streamData);
            }
            
            Options.AddOnlineTime();
            OnStreamStateUpdateAsync?.Invoke(Options.StreamState, streamData);
            OnStreamStateUpdate?.Invoke(Options.StreamState, streamData);
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while checking stream state. {e}");
        }
    }
}