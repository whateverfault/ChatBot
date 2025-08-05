using TwitchLib.Client.Events;

namespace ChatBot.bot.interfaces;

public abstract class CommandsHandler {
    public abstract void Handle(object? sender, OnChatCommandReceivedArgs args);
}