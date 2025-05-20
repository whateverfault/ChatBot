using ChatBot.Shared.interfaces;

namespace ChatBot.CLI.CliNodes;

public delegate State StateGetter();

public class CliNodeState: CliNode {
    public override string Text { get; }
    public override CliNodeType Type { get; }
    public StateGetter Value { get; set; }
    public ActionHandler Toggle { get; }
    

    public CliNodeState(string text, StateGetter value, ActionHandler toggle) {
        Type = CliNodeType.State;

        Text = text;
        Value = value;
        Toggle = toggle;
    }
}