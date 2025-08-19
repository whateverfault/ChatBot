namespace ChatBot.cli;

public class CliRenderer {
    private CliState? _state;

    private bool _forcedToRender;
    

    public void Bind(CliState state) {
        _state = state;
    }

    public Task Start() {
        if (_state == null) return Task.CompletedTask;
        RenderNodes();

        return Task.Run(() => {
                            while (true) {
                                switch (_forcedToRender) {
                                    case false when !Console.KeyAvailable:
                                        Thread.Sleep(50);
                                        continue;
                                    case false:
                                        int.TryParse(Console.ReadLine() ?? "0", out var index);
                                        _state.Cli.ActivateNode(index);
                                        break;
                                } 

                                Console.Clear();
                                RenderNodes();
                                _forcedToRender = false;
                            }  
                        });
    }

    public void ForceToRender() {
        _forcedToRender = true;
    }
    
    private void RenderNodes() {
        if (_state?.NodeSystem == null) return;
        
        var currentNodes = _state.NodeSystem.Current.Nodes;

        var index = 0;
        for (var i = 0; i < currentNodes.Count; i++, index++) {
            index -= currentNodes[i].PrintValue(index+1, out var end);
            Console.Write(end);
        }
    }
}