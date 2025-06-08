namespace ChatBot.CLI.CliNodes;

public class CliNodeIndex : CliNode {
    private readonly IntGetter _getter;
    private readonly IntSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeIndex(string text, IntGetter getter, CliNodePermission permission, IntSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        Console.Write($" - {_getter.Invoke()+1}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        Console.WriteLine($"Value: {_getter.Invoke()+1}");
        Console.Write("New Value: ");
        _setter.Invoke(int.Parse(Console.ReadLine() ?? "1")-1);
    }
}