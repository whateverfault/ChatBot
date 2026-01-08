using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

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

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        base.PrintValue(index, out end, drawIndex);
        IoHandler.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        IoHandler.WriteLine($"Value: {_getter.Invoke()}");
        
        var line = IoHandler.ReadLine("New Value: ");
        if (string.IsNullOrEmpty(line)) return;

        if (!float.TryParse(line, out var value)) return;
        _setter.Invoke(value);
    }
}