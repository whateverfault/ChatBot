using ChatBot.shared.interfaces;

namespace ChatBot.CLI.CliNodes;

public delegate State StateGetter();
public delegate void StateSetter();

public class CliNodeState : CliNode {
    private readonly StateGetter _getter;
    private readonly StateSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeState(string text, StateGetter getter, CliNodePermission permission, StateSetter? setter = null) {
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
        
        _setter.Invoke();
    }
}