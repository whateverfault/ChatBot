using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

public class CliNodeText : CliNode {
    private readonly bool _hasIndex;
    private readonly int _decrease;

    protected override string Text { get; }
    
    public override bool ShouldSkip { get; }
    

    public CliNodeText(string text, bool hasIndex = true, bool shouldSkip = true, int decrease = 0) {
        Text = text;
        _hasIndex = hasIndex;
        ShouldSkip = shouldSkip;
        _decrease = decrease;
    }

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        base.PrintValue(index, out end, drawIndex: false);
        return _decrease;
    }

    public override void Activate(CliState state) {}
}