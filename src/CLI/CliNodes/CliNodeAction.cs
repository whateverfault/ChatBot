using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

public delegate void ActionHandler();

public class CliNodeAction : CliNode {
    private readonly ActionHandler _action;
    
    protected override string Text { get; }


    public CliNodeAction(string text, ActionHandler action) {
        Text = text;
        _action = action;
    }

    public override int PrintValue(int index, out string end) {
        end = "\n";
        if (Text.Equals("Back")) {
            IoHandler.Write($"0. {Text}");
            return 1;
        }
        base.PrintValue(index, out end);
        return 0;
    }

    public override void Activate(CliState state) {
        _action.Invoke();
    }
}