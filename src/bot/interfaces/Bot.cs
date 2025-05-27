using ChatBot.Services.interfaces;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.bot.interfaces;

public abstract class Bot : Service {
    public override abstract Options Options { get; }
    public abstract event EventHandler<OnChatCommandReceivedArgs>? OnChatCommandReceived;
    public abstract event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public abstract event EventHandler<OnJoinedChannelArgs>? OnJoinedChannel;
    public abstract event EventHandler<OnConnectedArgs>? OnConnected;
    public abstract event EventHandler<OnLogArgs>? OnLog;


    public abstract void Start();

    public abstract void Enable();

    public abstract void Disable();

    public abstract ErrorCode TryGetClient(out ITwitchClient client);

    public abstract ITwitchClient GetClient();

    public override State GetServiceState() {
        return Options!.State;
    }
}