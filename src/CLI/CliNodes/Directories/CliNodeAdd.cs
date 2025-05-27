namespace ChatBot.CLI.CliNodes.Directories;

public delegate void AddHandler(string value);
public delegate void AddWithCommentHandler(string value, bool hasComment, string comment = "");

public class CliNodeAdd : CliNode {
    private readonly AddHandler _add;
    
    protected override string Text { get; }
    

    public CliNodeAdd(string text, AddHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Value: ");
        var value = Console.ReadLine() ?? "";
        
        _add.Invoke(value);
    }
}