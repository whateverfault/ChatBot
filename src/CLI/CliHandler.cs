using ChatBot.CLI.CliNodes;
using ChatBot.Shared.Handlers;
using ChatBot.twitchAPI;

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
            var currentNode = currentNodes[i];
            
            Console.Write($"{i+1}. {currentNode.Text}");
            switch (currentNode.Type) {
                case CliNodeType.Toggle: {
                    Console.Write($" - {((CliNodeToggle)currentNode).Value}");
                    break;
                }
                case CliNodeType.State: {
                    Console.Write($" - {((CliNodeState)currentNode).Value.Invoke()}");
                    break;
                }
                case CliNodeType.ActionWithInt: {
                    Console.Write($" - {((CliNodeActionWithInt)currentNode).Value.Invoke()}");
                    break;
                }
                case CliNodeType.Counter: {
                    Console.Write($" - {((CliNodeCounter)currentNode).Value.Invoke()}");
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
                var err = _data.Bot.GetClient(out var client);
                if (ErrorHandler.LogErrorAndWait(err)) {
                    break;
                }
                ((CliNodeCounter)node).Handle.Invoke(client, ((ChatBotOptions)_data.Bot.Options!).Channel!);
                ((CliNodeCounter)node).Increase();
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
                                                   new CliNodeCounter(
                                                                      "Increase Counter",
                                                                      _data.MessageRandomizer.GetCounter,
                                                                      _data.MessageRandomizer.HandleCounter,
                                                                      _data.MessageRandomizer.Options.IncreaseCounter
                                                                     ),
                                                   new CliNodeState(
                                                                    "Randomness", 
                                                                    _data.MessageRandomizer.GetRandomness,
                                                                    _data.MessageRandomizer.ToggleRandomness
                                                                   ),
                                                   new CliNodeState(
                                                                    "State", 
                                                                    _data.MessageRandomizer.GetServiceState,
                                                                    _data.MessageRandomizer.ToggleService
                                                                   ),
                                               ],
                                               DirectoryBack
                                               );
        
        var services = new CliNodeDirectory(
                                            "Services", 
                                            [
                                                gameReqsDir,
                                                randomMsgsDir,
                                            ],
                                            DirectoryBack
                                            );
        

        var rootDir = new CliNodeDirectory(
                                           "Root",
                                           [
                                               new CliNodeAction(
                                                                 "Login",
                                                                 _data.Bot.Login
                                                                ),
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