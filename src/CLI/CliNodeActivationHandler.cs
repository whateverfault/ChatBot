using ChatBot.bot.shared.handlers;

namespace ChatBot.cli;

public class CliNodeActivationHandler {
    private readonly CliState _state;


    public CliNodeActivationHandler(CliState state) {
        _state = state;
    }
    
    public void ActivateNode(int index) {
        if (_state.NodeSystem == null) return;
        
        var currentNodes = _state.NodeSystem.Current.Nodes;

        if (!(index >= 0 && index < currentNodes.Count)) {
            return;
        }

        IoHandler.Clear();
        if (index >= currentNodes.Count) return;
        currentNodes[index].Activate(_state);
    }
}