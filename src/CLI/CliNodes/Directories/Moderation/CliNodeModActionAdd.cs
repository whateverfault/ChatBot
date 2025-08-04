using ChatBot.services.moderation;
using ChatBot.shared.Handlers;

namespace ChatBot.cli.CliNodes.Directories.Moderation;

public delegate void AddModActionHandler(ModAction action);

public class CliNodeModActionAdd : CliNode {
    private readonly AddModActionHandler _add;

    protected override string Text { get; }
    

    public CliNodeModActionAdd(string text, AddModActionHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        var duration = 1;
        var warnCount = 1;
        var modComment = string.Empty;

        Console.Write("Name: ");
        var name = Console.ReadLine() ?? "Empty";
        
        var modActionType = AskModerationActionType();
        
        Console.Write("Global Filter Index: ");
        var line = Console.ReadLine() ?? "1";
        var index = int.Parse(string.IsNullOrWhiteSpace(line)? "1" : line)-1;
        
        var filters = state.Data.MessageFilter.GetFilters();
        if (index < 0 || index >= filters.Count) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
            return;
        }

        var action = modActionType switch {
                         ModerationActionType.Timeout         => new ModAction(name, index, duration, modComment, Restriction.Everyone),
                         ModerationActionType.Ban             => new ModAction(name, index, modComment, Restriction.Everyone),
                         ModerationActionType.Warn            => new ModAction(name, index, modComment, ModerationActionType.Warn, Restriction.Everyone),
                         ModerationActionType.WarnWithTimeout => new ModAction(name, index, duration, modComment, warnCount, Restriction.Everyone),
                         ModerationActionType.WarnWithBan     => new ModAction(name, index, modComment, warnCount, Restriction.Everyone),
                         _                                    => throw new ArgumentOutOfRangeException(),
                     };
        
        _add.Invoke(action);
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