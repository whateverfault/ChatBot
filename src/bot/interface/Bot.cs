using ChatBot.services.interfaces;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.bot.@interface;

public abstract class Bot : Service {
    public abstract override Options Options { get; }
    public abstract event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public abstract event EventHandler<OnLogArgs>? OnLog;


    public abstract void Start();

    public abstract void Stop();

    public abstract void Enable();

    public abstract void Disable();

    public abstract ErrorCode TryGetClient(out ITwitchClient? client);

    public abstract ITwitchClient? GetClient();
}