namespace ChatBot.cli.CliNodes;

public enum CliNodePermission {
    Default,
    ReadOnly,
}

public abstract class CliNode {
    protected abstract string Text { get; }

    public virtual bool ShouldSkip { get; }
    

    public virtual int PrintValue(int index, out string end) {
        end = "\n";
        Console.Write($"{index}. {Text}");
        return 0;
    }
    
    public abstract void Activate(CliState state);
}