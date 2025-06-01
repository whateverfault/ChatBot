namespace ChatBot.CLI.CliNodes.Directories;

public delegate void RemoveHandler(int index);

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
        var handled = string.IsNullOrEmpty(line)? "1" : line;
        var index = int.Parse(handled);
        if (index < 1) return;
        _remove.Invoke(index-1);
    }
}