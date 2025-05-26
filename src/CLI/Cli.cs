namespace ChatBot.CLI;

public class Cli {
    private readonly CliNodeHandler _nodeHandler;
    private readonly CliRenderer _cliRenderer;


    public Cli(CliData data) {
        var state = new CliState(data);
        var nodeSystem = new CliNodeSystem(state);
        
        state.Bind(nodeSystem);
        
        _nodeHandler = new CliNodeHandler(state);
        _cliRenderer = new CliRenderer(state);
        
        nodeSystem.InitNodes();
    }

    public void RenderNodes() {
        _cliRenderer.RenderNodes();
    }

    public void ActivateNode(int index) {
        _nodeHandler.ActivateNode(index);
    }
}