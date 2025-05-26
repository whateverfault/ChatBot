namespace ChatBot.CLI.CliNodes;

public delegate int IntGetter();
public delegate void IntSetter(int value);

public class CliNodeInt : CliNode {
    private readonly IntGetter _getter;
    private readonly IntSetter _setter;
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

    public override int PrintValue(int index) {
        base.PrintValue(index);
        Console.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        Console.Write("New Value: ");
        _setter.Invoke(int.Parse(Console.ReadLine() ?? "0"));
    }
}