﻿namespace ChatBot.cli.CliNodes.Directories.Presets;

public delegate void AddPreset(string name);

public class CliNodePresetAdd : CliNode {
    private readonly AddPreset _add;

    protected override string Text { get; }
    

    public CliNodePresetAdd(string text, AddPreset add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Name: ");
        var name = Console.ReadLine();
        if (name == null) return;
        
        _add.Invoke(name);
    }
}