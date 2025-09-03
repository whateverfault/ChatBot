using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.interfaces;

public abstract class Bot : Service {
    public abstract override Options Options { get; }
    public abstract event EventHandler<ChatMessage>? OnMessageReceived;


    public abstract Task StartAsync();
    public abstract void Start();

    public abstract void Stop();

    public abstract ErrorCode TryGetClient(out ITwitchClient? client);

    public abstract ITwitchClient? GetClient();
}