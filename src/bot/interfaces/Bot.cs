using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.api;
using TwitchAPI.client;
using TwitchAPI.client.commands.data;
using TwitchAPI.client.data;

namespace ChatBot.bot.interfaces;

public abstract class Bot : Service {
    public abstract TwitchApi Api { get; }
    
    public abstract override Options Options { get; }
    
    public abstract event EventHandler<RewardRedemption>? OnRewardRedeemed;
    public abstract event EventHandler<ChatMessage>? OnMessageReceived;
    public abstract event EventHandler<Command>? OnCommandReceived;


    public abstract Task Start();

    public abstract Task Stop();

    public abstract ErrorCode TryGetClient(out ITwitchClient? client);

    public abstract ITwitchClient? GetClient();
}