namespace ChatBot.cli;

public class CliRenderer {
    private readonly CliState _state;


    public CliRenderer(CliState state) {
        _state = state;
    }

    public void RenderNodes() {
        var currentNodes = _state.NodeSystem.Current.Nodes;

        var index = 0;
        for (var i = 0; i < currentNodes.Count; i++, index++) {
            index -= currentNodes[i].PrintValue(index+1, out var end);
            Console.Write(end);
        }
    }
}