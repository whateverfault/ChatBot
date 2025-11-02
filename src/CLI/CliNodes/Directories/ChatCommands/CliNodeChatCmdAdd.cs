using ChatBot.bot.services.chat_commands.data;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes.Directories.ChatCommands;

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
        Console.Write("Command Name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrEmpty(name)) return;
        
        Console.Write("Chat Output: ");
        var output = Console.ReadLine();
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