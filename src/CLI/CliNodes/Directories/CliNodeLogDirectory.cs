using ChatBot.bot.services.logger;

namespace ChatBot.cli.CliNodes.Directories;

public class CliNodeLogDirectory : CliNodeDirectory {
    private readonly CliState _state;
    
    protected override string Text { get; }

    public override List<CliNode> Nodes { get; }


    public CliNodeLogDirectory(string text, bool hasBackOption, CliState state, LoggerService loggerService) {
        Text = text;
        Nodes = [
                    new CliNodeAction(
                                      "Back",
                                      state.NodeSystem.DirectoryBack),
                    new CliNodeText(
                                    "-----------------------------------",
                                    false,
                                    true,
                                    1
                                   ),
                ];

        loggerService.OnLog += HandleLog;
        _state = state;
    }

    private void HandleLog(Log log) {
        Nodes.Add(
                  new CliNodeText(
                                  $"{log.Time.TimeOfDay.Hours:D2}:{log.Time.TimeOfDay.Minutes:D2}:{log.Time.TimeOfDay.Seconds:D2} [{log.Level}] {log.Message}",
                                  false
                                  )
                  );
        if (_state.NodeSystem.Current == this) {
            Program.ForceToRender();
        }
    }
}