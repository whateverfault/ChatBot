namespace ChatBot.CLI.CliNodes;

public delegate int EnumGetter();
public delegate void EnumNext();

public class CliNodeEnum : CliNode {
    private readonly EnumGetter _getter;
    private readonly Type _enumType;
    private readonly EnumNext? _next;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeEnum(string text, EnumGetter getter, Type enumType, CliNodePermission permission, EnumNext? next = null) {
        Text = text;
        _getter = getter;
        _enumType = enumType;
        _permission = permission;
        _next = next;
    }

    public override int PrintValue(int index, out string end) {
        end = "\n";
        var enumValue = _getter.Invoke();
        Console.Write($"{index}. {Text} - {Enum.ToObject(_enumType, enumValue)}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        _next?.Invoke();
    }
}