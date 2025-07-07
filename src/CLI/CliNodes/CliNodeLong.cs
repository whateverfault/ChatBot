namespace ChatBot.CLI.CliNodes;

public delegate long LongGetter();
public delegate void LongSetter(long value);

public class CliNodeLong : CliNode {
    private readonly LongGetter _getter;
    private readonly LongSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeLong(string text, LongGetter getter, CliNodePermission permission, LongSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        Console.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        Console.WriteLine($"Value: {_getter.Invoke()}");
        Console.Write("New Value: ");
        var line = Console.ReadLine() ?? "0";
        var index = long.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
        _setter.Invoke(index);
    }
}