using ChatBot.bot.services.logger;
using ChatBot.bot.services.logger.data;

namespace ChatBot.cli.CliNodes.Directories;

public class CliNodeLogDirectory : CliNodeDirectory {
    private const int MAX_LOGS = 50;
    
    private readonly CliState _state;
    
    protected override string Text { get; }

    private readonly Queue<CliNode> _nodesQueue;

    public sealed override List<CliNode> Nodes => _nodesQueue.ToList();


    public CliNodeLogDirectory(string text, bool hasBackOption, CliState state, LoggerService loggerService) {
        Text = text;
        _nodesQueue = [];

        if (state.NodeSystem != null) {
            if (hasBackOption) {
                _nodesQueue.Enqueue(
                                    new CliNodeAction(
                                                      "Back",
                                                      state.NodeSystem.DirectoryBack
                                                     )
                                   );
            }
        
            _nodesQueue.Enqueue(
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
        if (_nodesQueue.Count >= MAX_LOGS) {
            _nodesQueue.Dequeue();
        }
        
        _nodesQueue.Enqueue(
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