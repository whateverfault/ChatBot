using ChatBot.bot.interfaces;
using ChatBot.Services.chat_commands;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private Bot _bot = null!;
    private ITwitchClient Client => _bot.GetClient();

    public override string Name => ServiceName.MessageRandomizer;
    public override MessageRandomizerOptions Options { get; } = new();


    public void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.Match) return;
        
        if (message.Message.Length > 0
            && message.Message[0]
            == ((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).Options.CommandIdentifier) {
            _logger.Log(LogLevel.Info, "Message wasn't handled. Reason: Is Command");
            return;
        }
        HandleCounter(Client, message.Channel);
        var msg = new Message(message.Message, message.Username);
        if (Options.LoggerState == State.Disabled) {
            return;
        }
        Options.Logs.Add(msg);
        Options.Save();
    }

    public void HandleCounter(ITwitchClient client, string channel) {
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

        var err = GenerateRandomMessage(out var message);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        client.SendMessage(channel, message!.Msg);
    }

    private ErrorCode GenerateRandomMessage(out Message? message) {
        message = null;
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        if (Options.Logs.Count <= 0) {
            return ErrorCode.ListIsEmpty;
        }

        var randomIndex = Random.Shared.Next(0, Options.Logs.Count);
        Options.SetLastGeneratedMessage(Options.Logs[randomIndex]);
        Options.SetMessageState(MessageState.NotGuessed);
        _logger.Log(LogLevel.Info,
                    $"Message has been Generated: {Options.LastGeneratedMessage.Msg} | {Options.LastGeneratedMessage.Username}");
        
        Options.ZeroCounter();
        Options.IncreaseCounter();
        
        message = Options.LastGeneratedMessage;
        return ErrorCode.None;
    }

    public void GenerateAndSendRandomMessage(ITwitchClient client, string channel) {
        var err = GenerateRandomMessage(out var message);
        if (ErrorHandler.LogErrorAndPrint(err)) return;
        
        client.SendMessage(channel, message!.Msg);
    }

    public ErrorCode GetLastGeneratedMessage(out Message? message) {
        message = Options.LastGeneratedMessage;
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        return string.IsNullOrEmpty(Options.LastGeneratedMessage.Msg) ? ErrorCode.NotEnoughData : ErrorCode.None;
    }

    public override void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }

    public override State GetServiceState() {
        return Options.ServiceState;
    }
    
    public int GetLoggerStateAsInt() {
        return (int)Options.LoggerState;
    }

    public void LoggerStateNext() {
        Options.SetLoggerState((State)(((int)Options.LoggerState+1)%Enum.GetValues(typeof(State)).Length));
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

    public override void Init(Bot bot) {
        _bot = bot;

        if (Options.TryLoad()) {
            Options.SetRandomValue();
            return;
        }
        Options.SetDefaults();
        Options.SetRandomValue();
    }
}