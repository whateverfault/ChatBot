using ChatBot.Services.chat_commands;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.shared.Logging;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerService : Service {
    private Bot _bot = null!;
    private ITwitchClient Client => _bot.GetClient();

    public override MessageRandomizerOptions Options { get; } =  new();
    
    
    public void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        if (Options.State == State.Disabled) {
            ErrorHandler.LogError(ErrorCode.ServiceDisabled);
            return;
        }
        if (args.ChatMessage.Message.Length > 0 
            && args.ChatMessage.Message[0] == ((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).Options.CommandIdentifier) {
            Logger.Log(LogLevel.Info, "Message wasn't handled. Reason: Is Command");
            return;
        }
        HandleCounter(Client, args.ChatMessage.Channel);
        var msg = new Message(args.ChatMessage.Message, args.ChatMessage.Username);
        if (Options.LoggerState == State.Disabled) return;
        Options.Logs.Add(msg);
        Options.Save();
    }

    public void HandleCounter(ITwitchClient client, string channel) {
        if (Options.State == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return;
        }
        if ((Options.Randomness == State.Enabled && (Options.Counter < Options.RandomValue))
            || (Options.Randomness == State.Disabled && (Options.Counter < Options.CounterMax))) {
            Options.IncreaseCounter();
            return;
        }
        Options.ZeroCounter();
        Options.IncreaseCounter();
        if (Options.Randomness == State.Enabled) Options.SetRandomValue();

        var message = GenerateRandomMessage();
        if (message == null) return;
        client.SendMessage(channel, message.Msg);
    }

    private Message? GenerateRandomMessage() {
        if (Options.State == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return null;
        }
        if (Options.Logs.Count <= 0) {
            return null;
        }
        
        var randomIndex = Random.Shared.Next(0, Options.Logs.Count-1);
        Options.SetLastGeneratedMessage(Options.Logs[randomIndex]);
        Options.SetMessageState(MessageState.NotGuessed);
        Logger.Log(LogLevel.Info, $"Message has been Generated.\nMessage: {Options.LastGeneratedMessage.Msg} - {Options.LastGeneratedMessage.Username}");
        return Options.LastGeneratedMessage;
    }
    
    public void GenerateAndSendRandomMessage(ITwitchClient client, string channel) {
        if (Options.State == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return;
        }
        if (Options.Logs.Count <= 0) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidData);
            return;
        }
        client.SendMessage(channel, GenerateRandomMessage()!.Msg);
    }
    
    public ErrorCode GetLastGeneratedMessage(out Message? message) {
        message = Options.LastGeneratedMessage;
        if (Options.State == State.Disabled) return ErrorCode.ServiceDisabled;
        return string.IsNullOrEmpty(Options.LastGeneratedMessage.Msg)? ErrorCode.InvalidData : ErrorCode.None;
    }
    
    public override void ToggleService() {
        Options.SetState(Options.State == State.Enabled? State.Disabled : State.Enabled);
    }

    public override State GetServiceState() {
        return Options.State;
    }

    public State GetLoggerState() {
        return Options.LoggerState;
    }

    public void ToggleLoggerState() {
        Options.SetLoggerState(Options.LoggerState == State.Enabled? State.Disabled : State.Enabled);
    }
    
    public State GetRandomness() {
        return Options.Randomness;
    }
    
    public void ToggleRandomness() {
        if (Options.State == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return;
        }
        Options.SetRandomnessState(Options.Randomness == State.Enabled? State.Disabled : State.Enabled);
    }

    public int GetCounter() {
        return Options.Counter;
    }
    
    public override void Init(Bot bot) {
        _bot = bot;
        
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }
}