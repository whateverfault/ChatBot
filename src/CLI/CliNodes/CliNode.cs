namespace ChatBot.CLI.CliNodes;

public enum CliNodeType {
    Directory,
    Bool,
    Value,
    Action,
    WriteOnly,
    Counter,
    State,
    Client,
    ActionWithGetter,
}

public enum CliNodeValueType {
    String,
    Range,
    Int,
    Char,
    State
}

public enum CliNodePermission {
    Default,
    ReadOnly,
    WriteOnly,
}

public abstract class CliNode {
    public abstract string Text { get; }
    public abstract CliNodeType Type { get; }
    public virtual CliNodeValueType ValueType { get; }
    public abstract ActionHandler Action { get; }

    public virtual CliNodePermission Permission { get; }
    
    public abstract List<CliNode> Nodes { get; }
    public abstract bool HasBackOption { get; }
}