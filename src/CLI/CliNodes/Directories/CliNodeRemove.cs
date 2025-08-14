using ChatBot.bot.shared.Handlers;

namespace ChatBot.cli.CliNodes.Directories;

public delegate bool RemoveHandler(int index);

public class CliNodeRemove : CliNode {
    private readonly RemoveHandler _remove;
    
    protected override string Text { get; }


    public CliNodeRemove(string text, RemoveHandler remove) {
        Text = text;
        _remove = remove;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Index: ");
        
        var line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) {
            return;
        }
        
        var index = int.Parse(line);
        
        var result = _remove.Invoke(index-1);
        if (!result) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
        }
    }
}