using System.Text;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

public delegate List<string>? AliasesGetter();
public delegate void AliasesSetter(List<string> aliases);

public class CliNodeAliases : CliNode {
    private readonly AliasesGetter _getter;
    private readonly AliasesSetter _setter = null!;
    private readonly CliNodePermission _permission;

    protected override string Text { get; }


    public CliNodeAliases(string text, AliasesGetter getter, CliNodePermission permission, AliasesSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }
    
    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        IoHandler.Write($" - '{AliasesToString(_getter.Invoke())}'");
        return 0;
    }
    
    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;

        IoHandler.WriteLine($"Current Aliases: {AliasesToString(_getter.Invoke())}");
        
        var aliases = IoHandler.ReadLine("New Aliases: ");
        if (string.IsNullOrEmpty(aliases)) {
            return;
        }
        
        _setter.Invoke(AliasesFromString(aliases));
    }

    private string AliasesToString(List<string>? aliases) {
        if (aliases == null) {
            return string.Empty;
        }
        
        var sb = new StringBuilder();
        foreach (var alias in aliases) {
            sb.Append($"{alias} ");
        }

        return sb.ToString().TrimEnd();
    }

    private List<string> AliasesFromString(string aliases) {
        var splitted = aliases.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return splitted.ToList();
    }
}