using TwitchLib.Client.Interfaces;

namespace ChatBot.CLI.CliNodes;

public delegate void CounterHandler(ITwitchClient client, string channel);
public class CliNodeCounter : CliNode {
    public override string Text { get; }
    public override CliNodeType Type { get; }
    public IntGetter Value;
    public CounterHandler Handle { get; }
    public ActionHandler Increase { get; }


    public CliNodeCounter(string text, IntGetter value, CounterHandler handle, ActionHandler increase) {
        Type = CliNodeType.Counter;

        Text = text;
        Value = value;
        Handle = handle;
        Increase = increase;
    }
}