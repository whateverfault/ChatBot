using ChatBot.api.client;
using ChatBot.api.shared.requests;
using ChatBot.api.shared.requests.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker.Data;

namespace ChatBot.bot.services.stream_state_checker;

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
            var client = bot.GetClient();
            if (client?.Credentials == null) return; 
            
            var lastCheckedWasOnline = Options.StreamState.WasOnline;
            
            var streamResponse = await Requests.GetStreams(client.Credentials.Channel, client.Credentials, (_, message) => {
                                                               _logger.Log(LogLevel.Error, message);
                                                           });
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