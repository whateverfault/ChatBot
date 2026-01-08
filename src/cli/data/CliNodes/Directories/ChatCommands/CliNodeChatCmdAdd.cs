using ChatBot.bot.services.chat_commands.data;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes.Directories.ChatCommands;

public delegate void AddChatCmdHandler(CustomChatCommand chatCmd);

public class CliNodeChatCmdAdd : CliNode {
    private readonly AddChatCmdHandler _add;

    private int _id;

    protected override string Text { get; }


    public CliNodeChatCmdAdd(string text, AddChatCmdHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        var name = IoHandler.ReadLine("Command Name: ");
        if (string.IsNullOrEmpty(name)) return;
        
        var output = IoHandler.ReadLine("Chat Output: ");
        if (string.IsNullOrEmpty(output)) return;
        
        var chatCmd = new CustomChatCommand(
                                            _id++,
                                            name,
                                            "--",
                                            "--",
                                            true,
                                            [],
                                            output,
                                            Restriction.Everyone);
        _add.Invoke(chatCmd);
    }
}