using ChatBot.CLI.CliNodes;
using ChatBot.shared.interfaces;

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

            Console.Write($"{i+1}. {node.Text}");

            if (node.Permission == CliNodePermission.WriteOnly) {
                Console.WriteLine();
                continue;
            }
            
            switch (node.Type) {
                case CliNodeType.Client:
                case CliNodeType.ActionWithGetter:
                case CliNodeType.Value: {
                    var nodeValueType = node.ValueType;
                    switch (nodeValueType) {
                        case CliNodeValueType.String: {
                            Console.Write($" - {((CliNodeGeneric<string>)node).ValueGetter.Invoke()}");
                            break;
                        }
                        case CliNodeValueType.Range: {
                            Console.Write($" - {((CliNodeGeneric<Range>)node).ValueGetter.Invoke().Start.Value}..{((CliNodeGeneric<Range>)node).ValueGetter.Invoke().End.Value}");
                            break;
                        }
                        case CliNodeValueType.Int: {
                            Console.Write($" - {((CliNodeGeneric<int>)node).ValueGetter.Invoke()}");
                            break;
                        }
                        case CliNodeValueType.Char: {
                            Console.Write($" - '{((CliNodeGeneric<char>)node).ValueGetter.Invoke()}'");
                            break;
                        }
                        case CliNodeValueType.State: {
                            Console.Write($" - {((CliNodeGeneric<State>)node).ValueGetter.Invoke()}");
                            break;
                        }
                    }
                    break;
                }
            }

            Console.WriteLine();
        }
    }
}