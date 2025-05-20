using ChatBot.Services.interfaces;
using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerService : Service {
    private Message? _lastMessage;

    public override MessageRandomizerOptions Options { get; } =  new();
    
    
    public void HandleMessage(ChatMessage message, ITwitchClient client, string channel) {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogErrorAndWait(ErrorCode.ServiceDisabled);
            return;
        }
        HandleCounter(client, channel);
        Options.IncreaseCounter();
        var msg = new Message(message.Message, message.Username);
        Options.Logs.Add(msg);
    }

    public void HandleCounter(ITwitchClient client, string channel) {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogErrorAndWait(ErrorCode.ServiceDisabled);
            return;
        }
        if ((Options.Randomness != State.Enabled || (Options.Counter%Options.RandomValue != 0))
            && (Options.Randomness != State.Disabled || (Options.Counter%Options.CounterMax != 0))) {
            return;
        }
        Options.ZeroCounter();
        if (Options.Randomness == State.Enabled) Options.SetRandomValue();

        var message = GenerateRandomMessage();
        if (message == null) return;
        Console.WriteLine($"Message has been Generated.\nMessage: {message.Msg} - {message.Username}");
        client.SendMessage(channel, message.Msg);
            
        Options.Save();
    }

    public Message? GenerateRandomMessage() {
        if (Options.ServiceState == State.Disabled) ErrorHandler.LogErrorAndWait(ErrorCode.ServiceDisabled);
        if (Options.Logs.Count <= 0) {
            return null;
        }
        
        var randomIndex = Random.Shared.Next(0, Options.Logs.Count-1);
        _lastMessage= Options.Logs[randomIndex];

        return _lastMessage;
    }
    
    public ErrorCode GetLastMessage(out Message? message) {
        message = _lastMessage;
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        return _lastMessage == null ? ErrorCode.WrongInput : ErrorCode.None;
    }

    public override ErrorCode Enable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) return ErrorCode.PermDeny;
        if (Options.ServiceState == State.Enabled) return ErrorCode.AlreadyInState;
        
        Options.SetServiceState(State.Enabled);
        Options.Save();
        return ErrorCode.None;
    }

    public override ErrorCode Disable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) return ErrorCode.PermDeny;
        if (Options.ServiceState == State.Disabled) return ErrorCode.AlreadyInState;
        
        Options.SetServiceState(State.Disabled);
        Options.Save();
        return ErrorCode.None;
    }
    
    public override void ToggleService() {
        Options.SetServiceState(Options.ServiceState == State.Enabled? State.Disabled : State.Enabled);
    }

    public override State GetServiceState() {
        return Options.ServiceState;
    }

    public State GetRandomness() {
        return Options.Randomness;
    }
    
    public void ToggleRandomness() {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogErrorAndWait(ErrorCode.ServiceDisabled);
            return;
        }
        Options.SetRandomnessState(Options.Randomness == State.Enabled? State.Disabled : State.Enabled);
    }

    public int GetCounter() {
        return Options.Counter;
    }
    
    public override void Init() {
        if (Options.Load()) {
            Options.ZeroCounter();
            Options.SetRandomValue();
            return;
        }
        
        Options.SetDefaults();
        Options.ZeroCounter();
        Options.SetRandomValue();
    }
}