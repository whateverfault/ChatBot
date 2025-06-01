using ChatBot.Services.moderation;
using ChatBot.shared.Handlers;

namespace ChatBot.CLI.CliNodes.Directories;

public delegate void AddModActionHandler(ModAction action);

public class CliNodeModActionAdd : CliNode {
    private readonly AddModActionHandler _add;
    private readonly ModerationActionType _actionType;

    protected override string Text { get; }
    

    public CliNodeModActionAdd(string text, AddModActionHandler add, ModerationActionType actionType) {
        Text = text;
        _add = add;
        _actionType = actionType;
    }
    
    public override void Activate(CliState state) {
        var duration = 1;
        var warnCount = 1;
        var modComment = string.Empty;
        ModAction action;

        Console.Write("Global Pattern Index: ");
        var line = Console.ReadLine() ?? "0";
        var index = int.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
        var patterns = state.Data.MessageFilter.GetPatterns();
        if (index < 1 || index > patterns.Count) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
            return;
        };
        
        switch (_actionType) {
            case ModerationActionType.Timeout:
                action = new ModAction(index-1, duration, modComment, Restriction.Everyone);
                break;
            case ModerationActionType.Ban:
                action = new ModAction(index-1, modComment, Restriction.Everyone);
                break;
            case ModerationActionType.Warn:
                action = new ModAction(index-1, modComment, ModerationActionType.Warn, Restriction.Everyone);
                break;
            case ModerationActionType.WarnWithTimeout:
                action = new ModAction(index-1, duration, modComment, warnCount, Restriction.Everyone);
                break;
            case ModerationActionType.WarnWithBan:
                action = new ModAction(index-1, modComment, warnCount, Restriction.Everyone);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Console.Write("Now You Can Configure it by Going to 'Content->Your_Pattern'");
        _add.Invoke(action);
    }
}