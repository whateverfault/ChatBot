using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

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

    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        base.PrintValue(index, out end, drawIndex);
        IoHandler.Write($" - '{_getter.Invoke()}'");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;
        
        IoHandler.WriteLine($"Value: {_getter.Invoke()}");
        
        var line = IoHandler.ReadLine("New Value: ");
        if (string.IsNullOrEmpty(line)) {
            return;
        }
        
        _setter.Invoke(line[0]);
    }
}