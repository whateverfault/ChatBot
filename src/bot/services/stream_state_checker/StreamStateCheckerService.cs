using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.stream_state_checker;

public class StreamStateCheckerService : Service {
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
            
            var lastCheckedWasOnline = Options.StreamState.Online;
            
            var streamResponse = await TwitchChatBot.Instance.Api.GetStreams(client.Credentials.Broadcaster.Login, client.Credentials,
                                                                             (_, message) => { 
                                                                                 ErrorHandler.LogMessage(LogLevel.Error, message); 
                                                                             });
            if (streamResponse == null) {
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
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while checking stream state. {e.Data}");
        }
    }
}