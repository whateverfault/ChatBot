namespace ChatBot.cli.CliNodes;

public delegate int IntGetter();
public delegate void IntSetter(int value);

public class CliNodeInt : CliNode {
    private readonly IntGetter _getter;
    private readonly IntSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeInt(string text, IntGetter getter, CliNodePermission permission, IntSetter? setter = null) {
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
        
        var line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) {
            return;
        }

        if (!int.TryParse(line, out var value)) return;
        _setter.Invoke(value);
    }
}