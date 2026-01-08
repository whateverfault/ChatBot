using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.rendering;

public class CliOldRenderer : ICliRenderer {
    public void HandleInput(CliState state) {
        while (!IoHandler.KeyAvailable) {
            Thread.Sleep(50);
        }

        var line = IoHandler.ReadLine();
        if (!int.TryParse(line, out var index)) {
            index = 0;
        }
                                        
        state.Cli.ActivateNode(index);
    }
    
    public void Render(CliState state) {
        if (state.NodeSystem == null) return;
        
        var currentNodes = state.NodeSystem.Current.Nodes;

        var index = 0;
        for (var i = 0; i < currentNodes.Count; i++, index++) {
            index -= currentNodes[i].PrintValue(index+1, out var end, drawIndex: true);
            IoHandler.Write(end);
        }
    }
}