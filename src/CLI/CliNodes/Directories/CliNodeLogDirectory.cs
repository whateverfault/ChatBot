using ChatBot.Services.logger;

namespace ChatBot.CLI.CliNodes.Directories;

public enum LogType {
    NonTwitch,
    Twitch
}

public class CliNodeLogDirectory : CliNodeDirectory {
    private CliState _state;
    
    protected override string Text { get; }

    public override List<CliNode> Nodes { get; }


    public CliNodeLogDirectory(string text, bool hasBackOption, CliState state, LoggerService loggerService, LogType logType) {
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

        if (logType == LogType.NonTwitch) {
            loggerService.OnLog += HandleLog;
        } else if (logType == LogType.Twitch) {
            loggerService.OnTwitchLog += HandleLog;
        }

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