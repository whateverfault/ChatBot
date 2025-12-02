using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

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

        if (line.Equals("--")) {
            line = string.Empty;
        }

        _setter.Invoke(line);
    }
}