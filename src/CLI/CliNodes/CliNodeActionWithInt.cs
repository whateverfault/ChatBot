namespace ChatBot.CLI.CliNodes;

public delegate int IntGetter();
public class CliNodeActionWithInt : CliNode {
    public override CliNodeType Type { get; }
    public override string Text { get; }
    public IntGetter Value { get; }
    public ActionHandler? Action { get; }


    public CliNodeActionWithInt(string text, IntGetter value, ActionHandler? action) {
        Type = CliNodeType.ActionWithInt;
        
        Text = text;
        Value = value;
        Action = action;
    }
}