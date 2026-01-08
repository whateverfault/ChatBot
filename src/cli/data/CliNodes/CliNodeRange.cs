using ChatBot.bot.shared.handlers;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.cli.data.CliNodes;

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

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        base.PrintValue(index, out end, drawIndex);
        
        var range = _getter.Invoke();
        IoHandler.Write($" - {range.Start}..{range.End}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;

        var range = _getter.Invoke();
        IoHandler.WriteLine($"Value: {range.Start}..{range.End}");
        
        var line = IoHandler.ReadLine("from: ");
        
        var from = _getter.Invoke().Start;
        if (!string.IsNullOrEmpty(line)) {
            int.TryParse(line, out from);
        }        
        
        line = IoHandler.ReadLine("to: ");
        
        var to = _getter.Invoke().End;
        if (!string.IsNullOrEmpty(line)) {
            int.TryParse(line, out to);
        }   
        
        _setter.Invoke(new Range(from, to));
    }
}