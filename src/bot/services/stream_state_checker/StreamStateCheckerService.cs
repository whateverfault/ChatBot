using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.stream_state_checker;

public delegate void StreamStateChangedHandler(StreamState streamStateNew, StreamState streamStateOld, StreamData? streamData);
public delegate Task StreamStateChangedHandlerAsync(StreamState streamStateNew, StreamState streamStateOld, StreamData? streamData);
    
public delegate void StreamStateUpdatedHandler(StreamState streamState, StreamData? streamData);
public delegate Task StreamStateHandlerUpdatedAsync(StreamState streamState, StreamData? streamData);

public class StreamStateCheckerService : Service {
    public override StreamStateCheckerOptions Options { get; } = new StreamStateCheckerOptions();
    
    public event StreamStateUpdatedHandler? OnStreamStateUpdate;
    public event StreamStateHandlerUpdatedAsync? OnStreamStateUpdateAsync;
    
    public event StreamStateChangedHandler? OnStreamStateChanged;
    public event StreamStateChangedHandlerAsync? OnStreamStateChangedAsync;

    
    public async Task CheckState() {
        try {
            var bot = TwitchChatBot.Instance;
            var client = bot.GetClient();
            if (client?.Credentials == null) return; 
            
            var streamsStateOld = new StreamState(Options.StreamState);
            var lastCheckedWasOnline = Options.StreamState.Online;
            
            var streamResponse = await TwitchChatBot.Instance.Api.GetStreams(client.Credentials.Broadcaster.Login, client.Credentials,
                                                                             (_, message) => { 
                                                                                 ErrorHandler.LogMessage(LogLevel.Error, message); 
                                                                             });
            if (streamResponse == null) {
                Options.AddOfflineTime();
                if (lastCheckedWasOnline) {
                    OnStreamStateChangedAsync?.Invoke(Options.StreamState, streamsStateOld, null);
                    OnStreamStateChanged?.Invoke(Options.StreamState, streamsStateOld, null);
                    
                    ErrorHandler.LogMessage(LogLevel.Debug, $"{client.Credentials.Broadcaster.DisplayName} went offline");
                }
                
                OnStreamStateUpdateAsync?.Invoke(Options.StreamState, null);
                OnStreamStateUpdate?.Invoke(Options.StreamState, null);
                return;
            }
            
            var streamData = streamResponse.Data[0];
            Options.AddOnlineTime();
            
            if (!lastCheckedWasOnline) {
                OnStreamStateChangedAsync?.Invoke(Options.StreamState, streamsStateOld, streamData);
                OnStreamStateChanged?.Invoke(Options.StreamState, streamsStateOld, streamData);
                
                ErrorHandler.LogMessage(LogLevel.Debug, $"{client.Credentials.Broadcaster.DisplayName} went online");
            }
            
            OnStreamStateUpdateAsync?.Invoke(Options.StreamState, streamData);
            OnStreamStateUpdate?.Invoke(Options.StreamState, streamData);
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while checking stream state. {e.Data}");
        }
    }
}