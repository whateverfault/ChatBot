using ChatBot.CLI.CliNodes;
using ChatBot.shared.Handlers;
using Type = ChatBot.CLI.CliNodes.Type;

namespace ChatBot.CLI;

public class CliHandler {
    private readonly List<CliNodeDirectory> _directories = [];
    private int _dirPointer;
    private readonly CliData _data;
    

    public CliHandler(CliData data) {
        _data = data;
        InitNodes();
    }

    public void RenderNodes() {
        var currentNodes = _directories[_dirPointer].nodes;
        
        for (var i = 0; i < currentNodes.Length; i++) {
            var node = currentNodes[i];
            
            Console.Write($"{i+1}. {node.Text}");
            switch (node.Type) {
                case CliNodeType.Toggle: {
                    Console.Write($" - {((CliNodeToggle)node).Value}");
                    break;
                }
                case CliNodeType.State: {
                    Console.Write($" - {((CliNodeState)node).Value.Invoke()}");
                    break;
                }
                case CliNodeType.ActionWithInt: {
                    Console.Write($" - {((CliNodeActionWithInt)node).Value.Invoke()}");
                    break;
                }
                case CliNodeType.Counter: {
                    Console.Write($" - {((CliNodeCounter)node).Value.Invoke()}");
                    break;
                }
                case CliNodeType.Value: {
                    var nodeValueType = node.ValueType;
                    switch (nodeValueType) {
                        case Type.String: {
                            Console.Write($" - {((CliNodeValue<string>)node).ValueGetter.Invoke()}");
                            break;
                        }
                        case Type.Range: {
                            Console.Write($" - {((CliNodeValue<Range>)node).ValueGetter.Invoke().Start.Value}..{((CliNodeValue<Range>)node).ValueGetter.Invoke().End.Value}");
                            break;
                        }
                        case Type.Int: {
                            Console.Write($" - {((CliNodeValue<int>)node).ValueGetter.Invoke()}");
                            break;
                        }
                        case Type.Char: {
                            Console.Write($" - '{((CliNodeValue<char>)node).ValueGetter.Invoke()}'");
                            break;
                        }
                    }
                    break;
                }
            }

            Console.WriteLine();
        }
    }

    public void ActivateNode(int index) {
        var currentNodes = _directories[_dirPointer].nodes;
        
        if (!(index >= 0 && index < currentNodes.Length)) return;
        Console.Clear();
        
        var node = currentNodes[index];
        switch (node.Type) {
            case CliNodeType.Toggle: {
                ((CliNodeToggle)node).Value = !((CliNodeToggle)node).Value;
                break;
            }
            case CliNodeType.Action: {
                ((CliNodeAction)node).action?.Invoke();
                break;
            }
            case CliNodeType.Directory: {
                _directories.Add((CliNodeDirectory)node);
                _dirPointer++;
                break;
            }
            case CliNodeType.State: {
                ((CliNodeState)node).Toggle.Invoke();
                break;
            }
            case CliNodeType.ActionWithInt: {
                ((CliNodeActionWithInt)node).Action?.Invoke();
                break;
            }
            case CliNodeType.Counter: {
                var err = _data.Bot.TryGetClient(out var client);
                if (ErrorHandler.LogErrorAndPrint(err)) {
                    break;
                }
                ((CliNodeCounter)node).Handle.Invoke(client, _data.Bot.Options.Channel!);
                break;
            }
            case CliNodeType.Client: {
                var err = _data.Bot.TryGetClient(out var client);
                if (ErrorHandler.LogErrorAndPrint(err)) break;
                
                ((CliNodeClient)node).Action.Invoke(client, _data.Bot.Options.Channel!);
                break;
            }
            case CliNodeType.Value: {
                var nodeValueType = node.ValueType;
                switch (nodeValueType) {
                    case Type.String: {
                        var nodeValue = (CliNodeValue<string>)node;
                        if (nodeValue.IsReadOnly) break;
                        
                        Console.Write("Enter Value: ");
                        nodeValue.ValueSetter(Console.ReadLine() ?? "");
                        break;
                    }
                    case Type.Range: {
                        var nodeValue = (CliNodeValue<Range>)node;
                        if (nodeValue.IsReadOnly) break;
                        
                        Console.Write("From: ");
                        var line = Console.ReadLine() ?? "0";
                        var value = string.IsNullOrEmpty(line)? "0" : line;
                        var start = int.Parse(value);
                        
                        Console.Write("To: ");
                        line = Console.ReadLine() ?? "0";
                        value = string.IsNullOrEmpty(line)? "0" : line;
                        var end = int.Parse(value);

                        nodeValue.ValueSetter(new Range(start, end));
                        break;
                    }
                    case Type.Int: {
                        var nodeValue = (CliNodeValue<int>)node;
                        if (nodeValue.IsReadOnly) break;
                    
                        Console.Write("Enter Number: ");
                        var line = Console.ReadLine() ?? "0";
                        var value = string.IsNullOrEmpty(line)? "0" : line;
                        nodeValue.ValueSetter.Invoke(int.Parse(value));
                        break;
                    }
                    case Type.Char: {
                        var nodeValue = (CliNodeValue<char>)node;
                        if (nodeValue.IsReadOnly) break;
                    
                        Console.Write("Enter Character: ");
                        var ch = (char)Console.Read();
                        if (!char.IsSymbol(ch)) {
                            ErrorHandler.LogError(ErrorCode.WrongInput);
                            return;
                        }
                        nodeValue.ValueSetter(ch);
                        break;
                    }
                }
                break;
            }
        }
    }

    private void InitNodes() {
        var gameReqsDir = new CliNodeDirectory(
                                               "Game Requests",
                                               [
                                                   new CliNodeState(
                                                                   "State", 
                                                                   _data.GameRequests.GetServiceState,
                                                                   _data.GameRequests.ToggleService
                                                                  ),
                                               ],
                                               DirectoryBack
                                               );
        
        var randomMsgsDir = new CliNodeDirectory(
                                               "Random Messages",
                                               [
                                                   new CliNodeValue<int>(
                                                                      "Max Counter Value",
                                                                      _data.MessageRandomizer.Options.GetCounterMax,
                                                                      _data.MessageRandomizer.Options.SetCounterMax,
                                                                      Type.Int
                                                                     ),
                                                   new CliNodeValue<int>(
                                                                              "Random Value",
                                                                              _data.MessageRandomizer.Options.GetRandomValue,
                                                                              delegate{},
                                                                              Type.Int,
                                                                              true
                                                                              ),
                                                   new CliNodeCounter(
                                                                      "Increase Counter",
                                                                      _data.MessageRandomizer.GetCounter,
                                                                      _data.MessageRandomizer.HandleCounter
                                                                     ),
                                                   new CliNodeClient(
                                                                     "Generate Message",
                                                                     _data.MessageRandomizer.GenerateAndSendRandomMessage
                                                                    ),
                                                   new CliNodeState(
                                                                    "Counter Randomness", 
                                                                    _data.MessageRandomizer.GetRandomness,
                                                                    _data.MessageRandomizer.ToggleRandomness
                                                                   ),
                                                   new CliNodeValue<Range>(
                                                                      "Random Spreading", 
                                                                      _data.MessageRandomizer.Options.GetSpreading,
                                                                      _data.MessageRandomizer.Options.SetSpreading,
                                                                      Type.Range
                                                                     ),
                                                   new CliNodeState(
                                                                    "Collect Logs", 
                                                                    _data.MessageRandomizer.GetLoggerState,
                                                                    _data.MessageRandomizer.ToggleLoggerState
                                                                   ),
                                                   new CliNodeState(
                                                                    "State", 
                                                                    _data.MessageRandomizer.GetServiceState,
                                                                    _data.MessageRandomizer.ToggleService
                                                                   ),
                                               ],
                                               DirectoryBack
                                               );

        var chatCmdsDir = new CliNodeDirectory(
                                               "Chat Commands", 
                                               [
                                                   new CliNodeValue<char>(
                                                                          "Command Identifier",
                                                                          _data.ChatCommands.Options.GetCommandIdentifier,
                                                                          _data.ChatCommands.Options.SetCommandIdentifier,
                                                                          Type.Char
                                                                          ),
                                                   new CliNodeState(
                                                                    "Service State",
                                                                    _data.ChatCommands.GetServiceState,
                                                                    _data.ChatCommands.ToggleService
                                                                ),
                                                   ],
                                               DirectoryBack
                                               );
        
        var services = new CliNodeDirectory(
                                            "Services", 
                                            [
                                                chatCmdsDir,
                                                gameReqsDir,
                                                randomMsgsDir,
                                            ],
                                            DirectoryBack
                                            );


        var loginDir = new CliNodeDirectory(
                                            "Credentials",
                                            [
                                                new CliNodeAction(
                                                                  "Load",
                                                                  _data.Bot.Options.Load
                                                                 ),
                                                new CliNodeValue<string>(
                                                                         "Bot Username",
                                                                         _data.Bot.Options.GetUsername,
                                                                         _data.Bot.Options.SetUsername,
                                                                         Type.String
                                                                         ),
                                                new CliNodeValue<string>(
                                                                         "Channel",
                                                                         _data.Bot.Options.GetChannel,
                                                                         _data.Bot.Options.SetChannel,
                                                                         Type.String
                                                                        ),
                                                new CliNodeValue<string>(
                                                                         "OAuth",
                                                                         _data.Bot.Options.GetOAuth,
                                                                         _data.Bot.Options.SetOAuth,
                                                                         Type.String
                                                                        ),
                                                new CliNodeAction(
                                                                  "Save",
                                                                  _data.Bot.Options.Save
                                                                  ),
                                            ],
                                            DirectoryBack
                                           );
        
        var rootDir = new CliNodeDirectory(
                                           "Root",
                                           [
                                               loginDir,
                                               new CliNodeAction(
                                                                 "Initialize",
                                                                 _data.Bot.Start
                                                                ),
                                               new CliNodeState(
                                                                 "Enabled",
                                                                 _data.Bot.GetState,
                                                                 _data.Bot.Toggle
                                                                ),
                                               services,
                                           ]
                                          );
        
        _directories.Add(rootDir);
    }

    private void DirectoryBack() {
        if (_dirPointer > 0) {
            _directories.RemoveAt(_dirPointer--);
        }
    }
}