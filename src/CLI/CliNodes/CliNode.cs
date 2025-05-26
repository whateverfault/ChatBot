namespace ChatBot.CLI.CliNodes;

public enum CliNodePermission {
    Default,
    ReadOnly,
}

public abstract class CliNode {
    protected abstract string Text { get; }


    public virtual int PrintValue(int index) {
        Console.Write($"{index}. {Text}");
        return 0;
    }
    
    public abstract void Activate(CliState state);
}