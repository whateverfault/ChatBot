namespace ChatBot.CLI.CliNodes;

public enum CliNodeType {
    Directory,
    Toggle,
    Value,
    Action,
    ActionWithInt,
    Counter,
    State,
    Client,
}

public enum Type {
    String,
    Range,
    Int,
    Char,
}

public delegate void ActionHandler();

public abstract class CliNode {
    public abstract string Text { get; }
    public abstract CliNodeType Type { get; }
    public virtual Type ValueType { get; }
}