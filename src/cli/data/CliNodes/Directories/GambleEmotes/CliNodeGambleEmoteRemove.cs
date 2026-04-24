using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes.Directories.GambleEmotes;

public delegate bool RemoveGambleEmoteHandler(string name);

public class CliNodeGambleEmoteRemove : CliNode {
    private readonly RemoveGambleEmoteHandler _remove;
    
    public override string Text { get; }


    public CliNodeGambleEmoteRemove(string text, RemoveGambleEmoteHandler remove) {
        Text = text;
        _remove = remove;
    }
    
    public override void Activate(CliState state) {
        var name = IoHandler.ReadLine("Emote Name: ");
        if (string.IsNullOrEmpty(name)) return;
        
        var result = _remove.Invoke(name);
        if (!result) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
        }
    }
}