namespace ChatBot.CLI;

public class CliNodeHandler {
    private readonly CliState _state;


    public CliNodeHandler(CliState state) {
        _state = state;
    }
    
    public void ActivateNode(int index) {
        var currentNodes = _state.NodeSystem.Current.Nodes;

        if (!(index >= 0 && index < currentNodes.Count)) {
            return;
        }

        Console.Clear();
        currentNodes[index].Activate(_state);
    }
}