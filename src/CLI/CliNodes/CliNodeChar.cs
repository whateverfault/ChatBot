namespace ChatBot.cli.CliNodes;

public delegate char CharGetter();
public delegate void CharSetter(char value);

public class CliNodeChar : CliNode {
    private readonly CharGetter _getter;
    private readonly CharSetter _setter = null!;
    private readonly CliNodePermission _permission;
    
    protected override string Text { get; }


    public CliNodeChar(string text, CharGetter getter, CliNodePermission permission, CharSetter? setter = null) {
        Text = text;
        _getter = getter;
        _permission = permission;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        Console.Write($" - '{_getter.Invoke()}'");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        Console.WriteLine($"Value: {_getter.Invoke()}");
        Console.Write("New Value: ");
        
        var line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) {
            return;
        }
        
        _setter.Invoke(line[0]);
    }
}