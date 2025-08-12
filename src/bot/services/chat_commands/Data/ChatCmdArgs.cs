using ChatBot.api.client.commands.data;

namespace ChatBot.bot.services.chat_commands.Data;

public class ChatCmdArgs {
    public readonly Command Parsed;
    public ChatCommand Command { get; private set; } = null!;


    public ChatCmdArgs(Command parsed) {
        Parsed = parsed;
    }

    public void UpdateCommand(ChatCommand command) {
        Command = command;
    }
}