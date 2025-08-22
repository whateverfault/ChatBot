using ChatBot.api.twitch.client;
using ChatBot.api.twitch.client.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;

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