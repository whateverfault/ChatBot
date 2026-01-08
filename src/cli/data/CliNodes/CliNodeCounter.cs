using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

public class CliNodeCounter : CliNode {
    private readonly ActionHandler _increase;
    private readonly IntGetter _getter;
    
    protected override string Text { get; }


    public CliNodeCounter(string text, IntGetter getter, ActionHandler increase) {
        Text = text;
        _getter = getter;
        _increase = increase;
    }

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        base.PrintValue(index, out end, drawIndex);
        IoHandler.Write($"{Text} - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        _increase.Invoke();
    }
}