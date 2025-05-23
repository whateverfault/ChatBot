namespace ChatBot.CLI;

public class Cli {
    private readonly CliNodeHandler _nodeHandler;
    private readonly CliRenderer _cliRenderer;


    public Cli(CliData data) {
        var nodeSystem = new CliNodeSystem(data);
        
        _nodeHandler = new CliNodeHandler(nodeSystem, data);
        _cliRenderer = new CliRenderer(nodeSystem);
        
        nodeSystem.InitNodes();
    }

    public void RenderNodes() {
        _cliRenderer.RenderNodes();
    }

    public void ActivateNode(int index) {
        _nodeHandler.ActivateNode(index);
    }
}