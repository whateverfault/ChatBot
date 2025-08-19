namespace ChatBot.cli.CliNodes;

public delegate Task AsyncActionHandler();

public class CliNodeActionAsync : CliNode{
    private readonly AsyncActionHandler _action;
    
    protected override string Text { get; }


    public CliNodeActionAsync(string text, AsyncActionHandler action) {
        Text = text;
        _action = action;
    }

    public override int PrintValue(int index, out string end) {
        end = "\n";
        if (Text == "Back") {
            Console.Write($"0. {Text}");
            return 1;
        }
        base.PrintValue(index, out end);
        return 0;
    }

    public override void Activate(CliState state) {
        _action.Invoke();
    }
}