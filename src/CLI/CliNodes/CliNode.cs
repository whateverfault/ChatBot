namespace ChatBot.CLI.CliNodes;

public enum CliNodeType {
    Directory,
    Toggle,
    IntValue,
    StringValue,
    Action,
    ActionWithInt,
    Counter,
    State,
}

public delegate void ActionHandler();

public abstract class CliNode {
    public abstract string Text { get; }
    public abstract CliNodeType Type { get; }
}