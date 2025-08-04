using ChatBot.services.chat_commands.Parser.Data;

namespace ChatBot.services.chat_commands.Data;

public class ChatCmdArgs {
    public readonly ParsedChatCommand Parsed;
    public ChatCommand Command { get; private set; } = null!;


    public ChatCmdArgs(ParsedChatCommand parsed) {
        Parsed = parsed;
    }

    public void UpdateCommand(ChatCommand command) {
        Command = command;
    }
}