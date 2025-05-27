namespace ChatBot.CLI.CliNodes;

public delegate string StringGetter();
public delegate void StringSetter(string value);

public class CliNodeString : CliNode {
    private readonly StringGetter _getter;
    private readonly StringSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeString(string text, StringGetter getter, CliNodePermission permission, StringSetter? setter = null) {
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
        _setter.Invoke(Console.ReadLine() ?? "");
    }
}