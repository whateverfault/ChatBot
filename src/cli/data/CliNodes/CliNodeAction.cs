namespace ChatBot.cli.data.CliNodes;

public delegate void ActionHandler();

public class CliNodeAction : CliNode {
    private readonly ActionHandler _action;
    
    protected override string Text { get; }


    public CliNodeAction(string text, ActionHandler action) {
        Text = text;
        _action = action;
    }

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        index = (Text == "Back" ? 0 : index);
        
        base.PrintValue(index, out end, drawIndex);
        return Text == "Back" ? 1 : 0;
    }

    public override void Activate(CliState state) {
        _action.Invoke();
    }
}