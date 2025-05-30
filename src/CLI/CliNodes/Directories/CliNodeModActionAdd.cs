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
                Console.Write("Duration: ");
                line = Console.ReadLine() ?? "0";
                duration =  int.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
            
                Console.Write("Moderator Comment: ");
                modComment = Console.ReadLine() ?? "";

                action = new ModAction(index-1, duration, modComment);
                break;
            case ModerationActionType.Ban:
                Console.Write("Moderator Comment: ");
                modComment = Console.ReadLine() ?? "";

                action = new ModAction(index-1, modComment);
                break;
            case ModerationActionType.Warn:
                Console.Write("Moderator Comment: ");
                modComment = Console.ReadLine() ?? "";

                action = new ModAction(index-1, modComment, ModerationActionType.Warn);
                break;
            case ModerationActionType.WarnWithTimeout:
                Console.Write("Warn Count: ");
                line = Console.ReadLine() ?? "0";
                warnCount =  int.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
                
                Console.Write("Duration: ");
                line = Console.ReadLine() ?? "0";
                duration =  int.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
                
                action = new ModAction(index-1, duration, modComment, warnCount);
                break;
            case ModerationActionType.WarnWithBan:
                Console.Write("Warn Count: ");
                line = Console.ReadLine() ?? "0";
                warnCount =  int.Parse(string.IsNullOrWhiteSpace(line)? "0" : line);
                
                action = new ModAction(index-1, modComment, warnCount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _add.Invoke(action);
    }
}