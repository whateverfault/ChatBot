namespace ChatBot.CLI.CliNodes;

public class CliNodeText : CliNode {
    private readonly bool _hasIndex;
    private readonly int _decrease;

    protected override string Text { get; }

    public CliNodeText(string text, bool hasIndex = true, int decrease = 0) {
        _hasIndex = hasIndex;
        _decrease = decrease;
        Text = text;
    }

    public override int PrintValue(int index) {
        Console.Write($"{(_hasIndex? $"{index}. " : "")}{Text}");
        return _decrease;
    }

    public override void Activate(CliState state) {}
}