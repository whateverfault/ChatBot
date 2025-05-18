using TwitchLib.Client.Events;

namespace ChatBot.twitchAPI.interfaces;

public abstract class CommandsHandler {
    public abstract void Handle(object? sender, OnChatCommandReceivedArgs args);
}