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
        var index = int.Parse(Console.ReadLine() ?? "0");
        
        _remove.Invoke(index);
    }
}