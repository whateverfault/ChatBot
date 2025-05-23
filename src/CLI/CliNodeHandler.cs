using ChatBot.CLI.CliNodes;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

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
        switch (node.Type) {
            case CliNodeType.Bool: {
                ((CliNodeGeneric<bool>)node).ValueSetter.Invoke(!((CliNodeGeneric<bool>)node).ValueGetter.Invoke());
                break;
            }
            case CliNodeType.ActionWithGetter:
            case CliNodeType.Action: {
                node.Action.Invoke();
                break;
            }
            case CliNodeType.Directory: {
                _nodeSystem.DirectoryEnter(node);
                break;
            }
            case CliNodeType.Client: {
                var err = _data.Bot.TryGetClient(out var client);
                if (ErrorHandler.LogErrorAndPrint(err)) {
                    break;
                }
                ((CliNodeGeneric<int>)node).ClientAction.Invoke(client, _data.Bot.Options.Channel!);
                break;
            }
            case CliNodeType.Value: {
                var valueType = node.ValueType;
                switch (valueType) {
                    case CliNodeValueType.String: {
                        var currentNode = (CliNodeGeneric<string>)node;
                        if (!TrySetValue(currentNode, out var val)) {
                            return;
                        }
                        currentNode.ValueSetter(val);
                        break;
                    }
                    case CliNodeValueType.Range: {
                        var currentNode = (CliNodeGeneric<Range>)node;
                        if (!TrySetValue(currentNode, out var val)) {
                            return;
                        }
                        currentNode.ValueSetter(val);
                        break;
                    }
                    case CliNodeValueType.State: {
                        var currentNode = (CliNodeGeneric<State>)node;
                        if (!TrySetValue(currentNode, out var val)) {
                            return;
                        }
                        currentNode.ValueSetter(val);
                        break;
                    }
                    case CliNodeValueType.Int: {
                        var currentNode = (CliNodeGeneric<int>)node;
                        if (!TrySetValue(currentNode, out var val)) {
                            return;
                        }
                        currentNode.ValueSetter(val);
                        break;
                    }
                    case CliNodeValueType.Char: {
                        var currentNode = (CliNodeGeneric<char>)node;
                        if (!TrySetValue(currentNode, out var val)) {
                            return;
                        }
                        currentNode.ValueSetter(val);
                        break;
                    }
                }
                break;
            }
        }
    }    

     private bool TrySetValue<T>(CliNodeGeneric<T> nodeGeneric, out dynamic? val) {
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