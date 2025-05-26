namespace ChatBot.CLI.CliNodes;

public delegate void ActionHandler();

public class CliNodeAction : CliNode {
    private readonly ActionHandler _action;
    
    protected override string Text { get; }


    public CliNodeAction(string text, ActionHandler action) {
        Text = text;
        _action = action;
    }
    
    public override void Activate(CliState state) {
        _action.Invoke();
    }
}