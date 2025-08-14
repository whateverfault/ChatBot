using ChatBot.api.client;
using ChatBot.api.client.data;
using ChatBot.bot.services.interfaces;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.bot.interfaces;

public abstract class Bot : Service {
    public abstract override Options Options { get; }
    public abstract event EventHandler<ChatMessage>? OnMessageReceived;


    public abstract void Start();

    public abstract void Stop();

    public abstract ErrorCode TryGetClient(out ITwitchClient? client);

    public abstract ITwitchClient? GetClient();
}