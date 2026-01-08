namespace ChatBot.cli;

public class Cli {
    private readonly CliNodeActivationHandler _nodeActivationHandler;
    private readonly CliRenderer _renderer;
    
    public CliNodeSystem NodeSystem { get; }


    public Cli(CliData data) {
        var state = new CliState(this, data);
        NodeSystem = new CliNodeSystem(state);
        _renderer = new CliRenderer();
        
        state.Bind(NodeSystem, _renderer);
        _renderer.Bind(state);
        
        _nodeActivationHandler = new CliNodeActivationHandler(state);
        
        NodeSystem.InitNodes();
    }

    public Task StartRenderer() {
        return _renderer.Start();
    }

    public void ActivateNode(int index) {
        _nodeActivationHandler.ActivateNode(index);
    }
}