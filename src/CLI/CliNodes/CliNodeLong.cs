using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

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
        IoHandler.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        IoHandler.WriteLine($"Value: {_getter.Invoke()}");
        
        var line = IoHandler.ReadLine("New Value: ");
        if (string.IsNullOrEmpty(line)) {
            return;
        }
        
        if (!long.TryParse(line, out var value)) return;
        _setter.Invoke(value);
    }
}