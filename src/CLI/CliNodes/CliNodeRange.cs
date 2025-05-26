namespace ChatBot.CLI.CliNodes;

public delegate Range RangeGetter();
public delegate void RangeSetter(Range value);

public class CliNodeRange : CliNode {
    private readonly RangeGetter _getter;
    private readonly RangeSetter _setter;
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
    
    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;

        var from = int.Parse(Console.ReadLine() ?? "0");
        var to = int.Parse(Console.ReadLine() ?? "9999");
        
        _setter.Invoke(new Range(from, to));
    }
}