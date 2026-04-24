using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

public delegate double DoubleGetter();
public delegate void DoubleSetter(double value);

public class CliNodeDouble : CliNode {
    private readonly DoubleGetter _getter;
    private readonly DoubleSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    public override string Text { get; }


    public CliNodeDouble(string text, DoubleGetter getter, CliNodePermission permission, DoubleSetter? setter = null) {
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

        if (!double.TryParse(line, out var value)) return;
        _setter.Invoke(value);
    }
}