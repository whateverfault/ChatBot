using ChatBot.bot.services.logger;
using ChatBot.bot.services.logger.data;

namespace ChatBot.cli.data.CliNodes.Directories;

public class CliNodeLogDirectory : CliNodeDirectory {
    private const int MAX_LOGS = 50;
    
    private readonly CliState _state;
    
    protected override string Text { get; }

    public sealed override List<CliNode> Nodes { get; }
    public sealed override bool HasBackOption => true;


    public CliNodeLogDirectory(string text, CliState state, LoggerService loggerService) {
        Text = text;
        Nodes = [];

        if (state.NodeSystem != null) {
            Nodes.Add(
                          new CliNodeAction(
                                            "Back",
                                            state.NodeSystem.DirectoryBack
                                           )
                         );
        
            Nodes.Add(
                          new CliNodeText(
                                          "-----------------------------------",
                                          false,
                                          true,
                                          1
                                         )
                         );
        } 
        
        loggerService.OnLog += HandleLog;
        _state = state;
    }

    private void HandleLog(Log log) {
        if (Nodes.Count >= MAX_LOGS) {
            Nodes.RemoveAt(2);
        }
        
        Nodes.Add(
                      new CliNodeText(
                                      $"{log.Time.TimeOfDay.Hours:D2}:{log.Time.TimeOfDay.Minutes:D2}:{log.Time.TimeOfDay.Seconds:D2} [{log.Level}] {log.Message}",
                                      false
                                     )
                     );
        
        if (_state.NodeSystem == null
         || _state.NodeSystem.Current.Equals(this)) {
            _state.Renderer?.ForceToRender();
        }
    }
}