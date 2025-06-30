using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_commands.Data;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.CLI.CliNodes.Directories.ChatCommands;

public delegate void AddChatCmdHandler(ChatCommand chatCmd);

public class CliNodeChatCmdAdd : CliNode {
    private readonly AddChatCmdHandler _add;

    protected override string Text { get; }
    

    public CliNodeChatCmdAdd(string text, AddChatCmdHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Command Name: ");
        var name = Console.ReadLine() ?? "--";
        Console.Write("Chat Output: ");
        var output = Console.ReadLine() ?? "--";
        var chatCmd = new CustomChatCommand(
                                      name,
                                      "--",
                                      "--",
                                      [],
                                      output,
                                      Restriction.Everyone,
                                      State.Enabled,
                                      1,
                                      0);
        _add.Invoke(chatCmd);
    }
}