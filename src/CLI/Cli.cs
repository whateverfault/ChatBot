namespace ChatBot.cli;

public class Cli {
    private readonly CliNodeActivationHandler _nodeActivationHandler;
    private readonly CliRenderer _cliRenderer;


    public Cli(CliData data) {
        var state = new CliState(data);
        var nodeSystem = new CliNodeSystem(state);
        
        state.Bind(nodeSystem);
        
        _nodeActivationHandler = new CliNodeActivationHandler(state);
        _cliRenderer = new CliRenderer(state);
        
        nodeSystem.InitNodes();
    }

    public void RenderNodes() {
        _cliRenderer.RenderNodes();
    }

    public void ActivateNode(int index) {
        _nodeActivationHandler.ActivateNode(index);
    }
}