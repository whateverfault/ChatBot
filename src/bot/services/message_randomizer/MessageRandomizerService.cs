using ChatBot.api.client;
using ChatBot.api.client.data;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.bot.services.message_randomizer;

public class MessageRandomizerService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static ITwitchClient? Client => TwitchChatBot.Instance.GetClient();

    public override string Name => ServiceName.MessageRandomizer;
    public override MessageRandomizerOptions Options { get; } = new MessageRandomizerOptions();


    public void HandleChatLog(ChatMessage message) {
        if (Options.ServiceState == State.Disabled) return;
        HandleCounter(Client, message.Channel);
    }

    public void HandleCounter(ITwitchClient? client, string channel) {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return;
        }
        if (Options.Randomness == State.Enabled && Options.Counter < Options.RandomValue
            || Options.Randomness == State.Disabled && Options.Counter < Options.CounterMax) {
            Options.IncreaseCounter();
            return;
        }

        if (Options.Randomness == State.Enabled) {
            Options.SetRandomValue();
        }

        var err = Generate(out var message);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        client?.SendMessage(message!.Msg);
    }

    private ErrorCode Generate(out Message? message) {
        message = null;
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        
        var logs = Options.ChatLogsService.GetLogs();
        if (logs.Count <= 0) {
            return ErrorCode.ListIsEmpty;
        }

        var randomIndex = Random.Shared.Next(0, logs.Count);
        Options.SetLastGeneratedMessage(logs[randomIndex]);
        Options.SetMessageState(MessageState.NotGuessed);
        _logger.Log(LogLevel.Info,
                    $"Message has been Generated: {Options.LastGeneratedMessage.Msg} | {Options.LastGeneratedMessage.Username}");
        
        Options.ZeroCounter();
        Options.IncreaseCounter();
        
        message = Options.LastGeneratedMessage;
        return ErrorCode.None;
    }

    public void GenerateAndSend(ITwitchClient? client, string channel) {
        var err = Generate(out var message);
        if (ErrorHandler.LogError(err)
            || message == null) return;
        
        client?.SendMessage(message.Msg);
    }

    public ErrorCode GetLastGeneratedMessage(out Message? message) {
        message = Options.LastGeneratedMessage;
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        return string.IsNullOrEmpty(Options.LastGeneratedMessage.Msg)? ErrorCode.NotEnoughData : ErrorCode.None;
    }
    
    public override State GetServiceState() {
        return Options.ServiceState;
    }
    
    public int GetRandomnessAsInt() {
        return (int)Options.Randomness;
    }
    
    public void RandomnessNext() {
        Options.SetRandomnessState((State)(((int)Options.Randomness+1)%Enum.GetValues(typeof(State)).Length));
    }

    public int GetCounter() {
        return Options.Counter;
    }

    public override void Init() {
        base.Init();
        
        Options.SetRandomValue();
    }
}