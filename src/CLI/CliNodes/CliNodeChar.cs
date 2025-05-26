namespace ChatBot.CLI.CliNodes;

public delegate char CharGetter();
public delegate void CharSetter(char value);

public class CliNodeChar : CliNode {
    private readonly CharGetter _getter;
    private readonly CharSetter _setter;
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

    public override int PrintValue(int index) {
        base.PrintValue(index);
        Console.Write($" - '{_getter.Invoke()}'");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        Console.Write("New Value: ");
        _setter.Invoke((char)Console.Read());
    }
}