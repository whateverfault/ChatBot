namespace ChatBot.cli;

public class Cli {
    private readonly CliNodeActivationHandler _nodeActivationHandler;
    private readonly CliRenderer _renderer;


    public Cli(CliData data) {
        var state = new CliState(this, data);
        var nodeSystem = new CliNodeSystem(state);
        _renderer = new CliRenderer();
        
        state.Bind(nodeSystem, _renderer);
        _renderer.Bind(state);
        
        _nodeActivationHandler = new CliNodeActivationHandler(state);
        
        nodeSystem.InitNodes();
    }

    public Task StartRenderer() {
        return _renderer.Start();
    }

    public void ActivateNode(int index) {
        _nodeActivationHandler.ActivateNode(index);
    }
}