using TwitchLib.Client.Events;

namespace ChatBot.Services.chat_commands;

public class ChatCmdArgs {
    public OnChatCommandReceivedArgs Args { get; }
    public List<string> Parsed { get; }
    public bot.ChatBot Bot { get; }
    public ChatCommand Command { get; }

    
    public ChatCmdArgs(
        OnChatCommandReceivedArgs args,
        List<string> parsed,
        bot.ChatBot bot, ChatCommand command) {
        Args = args;
        Parsed = parsed;
        Bot = bot;
        Command = command;
    }
}