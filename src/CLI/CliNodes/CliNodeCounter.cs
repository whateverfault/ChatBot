using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

public class CliNodeCounter : CliNode {
    private readonly ActionHandler _increase;
    private readonly IntGetter _getter;
    
    protected override string Text { get; }


    public CliNodeCounter(string text, IntGetter getter, ActionHandler increase) {
        Text = text;
        _getter = getter;
        _increase = increase;
    }

    public override int PrintValue(int index, out string end) {
        end = "\n";
        IoHandler.Write($"{index}. {Text} - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        _increase.Invoke();
    }
}