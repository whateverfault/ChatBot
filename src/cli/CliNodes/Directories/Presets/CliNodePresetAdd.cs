using ChatBot.bot.services.moderation;

namespace ChatBot.cli.CliNodes.Directories.Presets;

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

    private ModerationActionType AskModerationActionType() {
        var modActionIndex = 0;

        var selected = (ModerationActionType)modActionIndex;
        
        Console.Clear();
        Console.Write($"Type (Space -> next): {selected}");
        
        while (true) {
            if (!Console.KeyAvailable) {
                continue;
            }

            var pressed = Console.ReadKey(true);
            switch (pressed.Key) {
                case ConsoleKey.Spacebar:
                    selected = (ModerationActionType)(++modActionIndex%Enum.GetValues(typeof(ModerationActionType)).Length);
                    Console.Clear();
                    Console.Write($"Type (Space -> next): {selected}");
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    return selected;
            }
        }
    }
}