using ChatBot.bot.services.moderation;
using ChatBot.bot.services.moderation.data;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes.Directories.Moderation;

public delegate void AddModActionHandler(ModAction action);

public class CliNodeModActionAdd : CliNode {
    private readonly AddModActionHandler _add;

    protected override string Text { get; }
    

    public CliNodeModActionAdd(string text, AddModActionHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        const int duration = 1;
        const int warnCount = 1;
        var modComment = string.Empty;
        
        var name = IoHandler.ReadLine("Name: ");
        if (string.IsNullOrEmpty(name)) return;
        
        var modActionType = AskModerationActionType();
        var line = IoHandler.ReadLine("Global Filter Index: ");
        
        if (string.IsNullOrEmpty(line)) return;
        if (int.TryParse(line, out var index)) return;

        --index;
        
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
        
        IoHandler.ClearWrite($"Type (Space -> next): {selected}");
        
        while (true) {
            if (!IoHandler.KeyAvailable) {
                Thread.Sleep(50);
                continue;
            }

            var pressed = IoHandler.ReadKey(true);
            switch (pressed.Key) {
                case ConsoleKey.Spacebar:
                    selected = (ModerationActionType)(++modActionIndex%Enum.GetValues(typeof(ModerationActionType)).Length);
                    IoHandler.ClearWrite($"Type (Space -> next): {selected}");
                    break;
                case ConsoleKey.Enter:
                    IoHandler.Clear();
                    return selected;
            }
        }
    }
}