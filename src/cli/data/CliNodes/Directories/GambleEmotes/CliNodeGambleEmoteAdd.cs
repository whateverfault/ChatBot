using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.cli.data.CliNodes.Directories.GambleEmotes;

public delegate bool AddGambleEmoteHandler(string name);

public class CliNodeGambleEmoteAdd : CliNode {
    private readonly AddGambleEmoteHandler _addHandler;
    
    public override string Text { get; }

    
    public CliNodeGambleEmoteAdd(string text, AddGambleEmoteHandler addHandler) {
        Text = text;
        _addHandler = addHandler;
    }
    
    public override void Activate(CliState state) {
        var name = IoHandler.ReadLine("Enter Name: ");
        if (string.IsNullOrEmpty(name)) return;

        if (_addHandler.Invoke(name)) {
            return;
        }

        ErrorHandler.PrintMessage(LogLevel.Error, $"Failed To Add New Emote '{name}'");
    }
}