using ChatBot.shared.Handlers;

namespace ChatBot.CLI.CliNodes;

public delegate int IndexGetter();
public delegate bool IndexSetter(int index);

public class CliNodeIndex : CliNode {
    private readonly IndexGetter _getter;
    private readonly IndexSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeIndex(string text, IndexGetter getter, CliNodePermission permission, IndexSetter? setter = null) {
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
        var line = Console.ReadLine() ?? "1";
        var index = int.Parse(string.IsNullOrWhiteSpace(line)? "1" : line);
        var result = _setter.Invoke(index-1);

        if (!result) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
        }
    }
}