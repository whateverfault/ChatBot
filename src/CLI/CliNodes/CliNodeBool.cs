using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

public delegate bool BoolGetter();
public delegate void BoolSetter(bool value);

public class CliNodeBool : CliNode {
    private readonly BoolGetter _getter;
    private readonly BoolSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeBool(string text, BoolGetter getter, CliNodePermission permission, BoolSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        IoHandler.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        _setter.Invoke(!_getter.Invoke());
    }
}