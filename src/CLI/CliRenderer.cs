using ChatBot.CLI.CliNodes;

namespace ChatBot.CLI;

public class CliRenderer {
    private readonly CliNodeSystem _nodeSystem;


    public CliRenderer(CliNodeSystem nodeSystem) {
        _nodeSystem = nodeSystem;
    }

    public void RenderNodes() {
        var currentNodes = _nodeSystem.Current.Nodes;

        for (var i = 0; i < currentNodes.Count; i++) {
            var node = currentNodes[i];
            
            RenderNode(node, i);
        }
    }
    
    private void RenderNode(CliNode node, int index) {
        Console.Write($"{index+1}. {node.Text}");

        if (node.Permission == CliNodePermission.WriteOnly) {
            Console.WriteLine();
            return;
        }
            
        switch (node.Type) {
            case CliNodeType.Client:
            case CliNodeType.ActionWithGetter:
            case CliNodeType.Value: {
                var nodeValueType = node.ValueType;
                switch (nodeValueType) {
                    case CliNodeValueType.State:
                    case CliNodeValueType.Char:
                    case CliNodeValueType.Int:
                    case CliNodeValueType.String: {
                        Console.Write($" - {node.ValueGetter.Invoke()}");
                        break;
                    }
                    case CliNodeValueType.Range: {
                        Console.Write($" - {node.ValueGetter.Invoke().Start.Value}..{node.ValueGetter.Invoke().End.Value}");
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            }
        }

        Console.WriteLine();
    }
}