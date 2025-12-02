using ChatBot.bot.shared.handlers;

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
                                    case false when !IoHandler.KeyAvailable:
                                        Thread.Sleep(50);
                                        continue;
                                    case false:
                                        var line = IoHandler.ReadLine();
                                        if (!int.TryParse(line, out var index)) {
                                            index = 0;
                                        }
                                        
                                        _state.Cli.ActivateNode(index);
                                        break;
                                } 

                                IoHandler.Clear();
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
            IoHandler.Write(end);
        }
    }
}