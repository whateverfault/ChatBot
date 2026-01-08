using ChatBot.bot.shared.handlers;
using ChatBot.cli.data.CliNodes;

namespace ChatBot.cli.data.rendering;

public class CliNewRenderer : ICliRenderer {
    private const int SCOPE = 10;
    private int _pointer = 1;
    
    
    public void HandleInput(CliState state) {
        while (!IoHandler.KeyAvailable) {
            Thread.Sleep(50);
        }
        
        var keyInfo = IoHandler.ReadKey(true);
        var nodes = state.Cli.NodeSystem.Current.Nodes;

        switch (keyInfo.Key) {

            case ConsoleKey.W:
            case ConsoleKey.UpArrow: {
                PointerUp(state);
                
                while (nodes[_pointer] is CliNodeText) {
                    PointerUp(state);
                }
                
                break;
            }

            case ConsoleKey.S:
            case ConsoleKey.DownArrow: {
                PointerDown(state);
                
                while (nodes[_pointer] is CliNodeText) {
                    PointerDown(state);
                }
                break;
            }

            case ConsoleKey.E:
            case ConsoleKey.F:
            case ConsoleKey.D:
            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter: {
                state.Cli.ActivateNode(_pointer);

                if (!nodes.Equals(state.NodeSystem?.Current.Nodes)) {
                    _pointer = 0;
                }
                break;
            }
        }
        
        if (!state.Cli.NodeSystem.Current.HasBackOption
         && _pointer == 0) {
            _pointer = 1;
        }
    }
    
    public void Render(CliState state) {
        if (state.NodeSystem == null) return;
        
        var currentNodes = state.NodeSystem.Current.Nodes;


        var low = Math.Max(_pointer - SCOPE, 0);
        var high = Math.Min(_pointer + SCOPE, currentNodes.Count);
        
        var index = low;
        
        for (var i = low; i < currentNodes.Count && index < high; ++i, ++index) {
            if (index == _pointer) {
                IoHandler.BackgroundColor = ConsoleColor.Gray;
                IoHandler.ForegroundColor = ConsoleColor.Black;
            }
            
            currentNodes[i].PrintValue(index+1, out var end);
            IoHandler.Write(end);
            
            IoHandler.BackgroundColor = ConsoleColor.Black;
            IoHandler.ForegroundColor = ConsoleColor.Gray;

            if (index == high - 1
             && i < currentNodes.Count - 1
             && currentNodes[i + 1] is CliNodeText) {
                ++high;
            }
        }
    }

    private void PointerUp(CliState state) {
        var nodesCount = state.Cli.NodeSystem.Current.Nodes.Count;
        
        _pointer = (_pointer - 1) % nodesCount;

        if (!state.Cli.NodeSystem.Current.HasBackOption
         && _pointer == 0) {
            _pointer = nodesCount - 1;
        }

        if (_pointer < 0) {
            _pointer = nodesCount - 1;
        }
    }

    private void PointerDown(CliState state) {
        var nodesCount = state.Cli.NodeSystem.Current.Nodes.Count;
        
        _pointer = (_pointer + 1) % nodesCount;
    }
}