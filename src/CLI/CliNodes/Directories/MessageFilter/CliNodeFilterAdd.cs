using ChatBot.bot.services.message_filter;

namespace ChatBot.cli.CliNodes.Directories.MessageFilter;

public delegate void AddFilterHandler(Filter filter);

public class CliNodeFilterAdd : CliNode {
    private readonly AddFilterHandler _addHandler;
    
    protected override string Text { get; }

    
    public CliNodeFilterAdd(string text, AddFilterHandler addHandler) {
        Text = text;
        _addHandler = addHandler;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Enter Name: ");
        var name = Console.ReadLine() ?? "Empty";
        
        Console.Write("Enter Pattern: ");
        var pattern = Console.ReadLine() ?? "Empty";

        var filter = new Filter(name, pattern);
        _addHandler.Invoke(filter);
    }
}