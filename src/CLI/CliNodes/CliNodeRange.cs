namespace ChatBot.CLI.CliNodes;

public delegate Range RangeGetter();
public delegate void RangeSetter(Range value);

public class CliNodeRange : CliNode {
    private readonly RangeGetter _getter;
    private readonly RangeSetter _setter = null!;
    private readonly CliNodePermission _permission;


    protected override string Text { get; }


    public CliNodeRange(string text, RangeGetter getter, CliNodePermission permission, RangeSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        
        var range = _getter.Invoke();
        Console.Write($" - {range.Start.Value}..{range.End.Value}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;

        var range = _getter.Invoke();
        Console.WriteLine($"Value: {range.Start.Value}..{range.End.Value}");
        
        Console.Write("from: ");
        var line = Console.ReadLine();
        var handled = string.IsNullOrEmpty(line)? "0" : line;
        var from = int.Parse(handled);
        
        Console.Write("to: ");
        line = Console.ReadLine();
        handled = string.IsNullOrEmpty(line)? "9999" : line;
        var to = int.Parse(handled);
        
        _setter.Invoke(new Range(from, to));
    }
}