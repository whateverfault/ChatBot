namespace ChatBot.CLI.CliNodes;

public class CliNodeAction : CliNode {
    public override CliNodeType Type { get; }
    public override string Text { get; }
    
    public readonly ActionHandler? action;


    public CliNodeAction(string text, ActionHandler? action) {
        Type = CliNodeType.Action;
        
        Text = text;
        this.action = action;
    }
}