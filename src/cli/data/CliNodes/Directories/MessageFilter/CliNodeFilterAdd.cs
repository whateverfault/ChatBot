using ChatBot.bot.services.message_filter.data;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes.Directories.MessageFilter;

public delegate void AddFilterHandler(Filter filter);

public class CliNodeFilterAdd : CliNode {
    private readonly AddFilterHandler _addHandler;
    
    protected override string Text { get; }

    
    public CliNodeFilterAdd(string text, AddFilterHandler addHandler) {
        Text = text;
        _addHandler = addHandler;
    }
    
    public override void Activate(CliState state) {
        var name = IoHandler.ReadLine("Enter Name: ");
        if (string.IsNullOrEmpty(name)) return;
        
        var pattern = IoHandler.ReadLine("Enter Pattern: ");
        if (string.IsNullOrEmpty(pattern)) return;
        
        var filter = new Filter(name, pattern);
        _addHandler.Invoke(filter);
    }
}