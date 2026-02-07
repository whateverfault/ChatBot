using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared.handlers;
using TwitchAPI.api.data.requests;
using TwitchAPI.client;

namespace ChatBot.bot.services.bot_lifecycle;

public class BotLifecycleService : Service {
    private readonly object _sync = new object();

    private readonly TimeSpan _timeThreshold = TimeSpan.FromSeconds(30);
    private Task? _startTask;
    private Task? _stopTask;
    
    public override BotLifecycleOptions Options { get; } = new BotLifecycleOptions();


    public Task BotStart(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        if (Options.ServiceState == State.Disabled
         || !streamStateNew.Online) {
            return Task.CompletedTask;
        }

        lock (_sync) {
            if (BotOnline() || _startTask != null)
                return _startTask ?? Task.CompletedTask;
            
            _startTask = Task.Run(async () => {
                                      try {
                                          var startTask = TwitchChatBot.Instance.Start();
                                          var timeoutTask = Task.Delay(_timeThreshold);

                                          await Task.WhenAny(startTask, timeoutTask);
                                          if (!BotOnline()) {
                                              ErrorHandler.LogMessage(LogLevel.Error, "Failed to start the bot within a time limit.");
                                          }
                                      }
                                      catch (Exception ex) {
                                          ErrorHandler.LogMessage(LogLevel.Error, ex.Message);
                                      }
                                      finally {
                                          lock (_sync) {
                                              _startTask = null;
                                          }
                                      }
                                  });
        }
        
        return _startTask;
    }
    
    public Task BotStop(StreamState streamState, StreamData? streamData) {
        if (Options.ServiceState == State.Disabled
         || streamState.Online 
         || streamState.OfflineTime < Options.GetDisconnectTimeout()) {
            return Task.CompletedTask;
        }

        lock (_sync) {
            if (!BotOnline() || _stopTask != null)
                return _stopTask ?? Task.CompletedTask;
            
            _stopTask = Task.Run(async () => {
                                     try {
                                         var stopTask = TwitchChatBot.Instance.Stop();
                                         var timeoutTask = Task.Delay(_timeThreshold);

                                         await Task.WhenAny(stopTask, timeoutTask);
                                         if (BotOnline()) {
                                             ErrorHandler.LogMessage(LogLevel.Error, "Failed to stop the bot within a time limit.");
                                         }
                                     }
                                     catch (Exception ex) {
                                         ErrorHandler.LogMessage(LogLevel.Error, ex.Message);
                                     }
                                     finally {
                                         lock (_sync) {
                                             _stopTask = null;
                                         }
                                     }
                                 });
        }
        
        return _stopTask;
    }

    public bool BotOnline() {
        return TwitchChatBot.Instance.Online;
    }
}