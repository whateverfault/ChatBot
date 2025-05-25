using ChatBot.CLI.CliNodes;
using ChatBot.shared.Handlers;

namespace ChatBot.CLI;

public class CliNodeHandler {
    private readonly CliNodeSystem _nodeSystem;
    private readonly CliData _data;


    public CliNodeHandler(CliNodeSystem nodeSystem, CliData data) {
        _nodeSystem = nodeSystem;
        _data = data;
    }
    
    public void ActivateNode(int index) {
        var currentNodes = _nodeSystem.Current.Nodes;

        if (!(index >= 0 && index < currentNodes.Count)) {
            return;
        }

        Console.Clear();

        var node = currentNodes[index];
        TrySetValue(node, out var val);
        
        switch (node.Type) {
            case CliNodeType.Bool: {
                node.ValueSetter.Invoke(!node.ValueGetter.Invoke());
                break;
            }
            case CliNodeType.ActionWithGetter:
            case CliNodeType.Action: {
                node.Action.Invoke();
                break;
            }
            case CliNodeType.DynamicDirectory:
            case CliNodeType.Directory: {
                _nodeSystem.DirectoryEnter(node);
                break;
            }
            case CliNodeType.Client: {
                var err = _data.Bot.TryGetClient(out var client);
                if (ErrorHandler.LogErrorAndPrint(err)) {
                    break;
                }
                node.ClientAction.Invoke(client, _data.Bot.Options.Channel!);
                break;
            }
            case CliNodeType.Value: {
                node.ValueSetter(val);
                break;
            }
            case CliNodeType.NodeRemover: {
                var rem = int.Parse(string.IsNullOrEmpty(val) ? "1" : val);
                node.NodeRemove(rem);
                break;
            }
            case CliNodeType.NodeAdder: {
                node.NodeAppend(new CliNode(val));
                break;
            }
        }
    }
    
     private bool TrySetValue(CliNode nodeGeneric, out dynamic? val) {
        val = null;
        if (nodeGeneric.Permission == CliNodePermission.ReadOnly) {
            return false;
        }
        var valueType = nodeGeneric.ValueType;
                switch (valueType) {
                    case CliNodeValueType.String: {
                        Console.Write("Enter Value: ");
                        val = Console.ReadLine() ?? "";
                        break;
                    }
                    case CliNodeValueType.Range: {
                        Console.Write("From: ");
                        var line = Console.ReadLine() ?? "0";
                        var value = string.IsNullOrEmpty(line) ? "0" : line;
                        var start = int.Parse(value);

                        Console.Write("To: ");
                        line = Console.ReadLine() ?? "0";
                        value = string.IsNullOrEmpty(line) ? "0" : line;
                        var end = int.Parse(value);

                        val = new Range(start, end);
                        break;
                    }
                    case CliNodeValueType.Int: {
                        Console.Write("Enter Number: ");
                        var line = Console.ReadLine() ?? "0";
                        var value = string.IsNullOrEmpty(line) ? "0" : line;
                        val = int.Parse(value);
                        break;
                    }
                    case CliNodeValueType.Char: {
                        Console.Write("Enter Character: ");
                        var ch = (char)Console.Read();
                        if (!char.IsSymbol(ch)) {
                            ErrorHandler.LogError(ErrorCode.WrongInput);
                            break;
                        }
                        val = ch;
                        break;
                    }
                }
         return true;
    }
}