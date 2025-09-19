namespace ChatBot.cli.CliNodes;

public delegate float FloatGetter();
public delegate void FloatSetter(float value);

public class CliNodeFloat : CliNode {
    private readonly FloatGetter _getter;
    private readonly FloatSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeFloat(string text, FloatGetter getter, CliNodePermission permission, FloatSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) _setter = setter;
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
        if (string.IsNullOrEmpty(line)) return;

        if (!float.TryParse(line, out var value)) return;
        _setter.Invoke(value);
    }
}