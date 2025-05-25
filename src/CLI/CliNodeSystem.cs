using ChatBot.CLI.CliNodes;

namespace ChatBot.CLI;

public class CliNodeSystem {
    private readonly CliData _data;
    private readonly List<CliNode> _directories;
    private int _dirPointer;


    public CliNodeSystem(CliData data) {
        _data = data;
        _directories = [];
    }

    public CliNode Current => _directories[_dirPointer];
    
    public void DirectoryEnter(CliNode dir) {
        _directories.Add(dir);
        _dirPointer++;
    }

    private void DirectoryBack() {
        if (_dirPointer > 0) {
            _directories.RemoveAt(_dirPointer--);
        }
    }

    public void InitNodes() {
        var gameReqsDir = new CliNode(
                                      "Game Requests",
                                      DirectoryBack, 
                                      [
                                          new CliNode(
                                                   "State",
                                                   _data.GameRequests.GetServiceStateDynamic,
                                                   _data.GameRequests.ToggleService,
                                                   CliNodeValueType.State
                                                   )
                                      ]);

        var randomMsgsDir = new CliNode(
                                        "Random Messages",
                                        DirectoryBack, 
                                        [
                                            new CliNode(
                                                        "Max Counter Value",
                                                        _data.MessageRandomizer.Options.GetCounterMaxDynamic, 
                                                        _data.MessageRandomizer.Options.SetCounterMaxDynamic, 
                                                        CliNodeValueType.Int
                                                        ),
                                            new CliNode(
                                                        "Random Value", 
                                                        _data.MessageRandomizer.Options.GetRandomValueDynamic,
                                                        CliNodeValueType.Int
                                                        ),
                                            new CliNode(
                                                        "Increase Counter",
                                                        _data.MessageRandomizer.GetCounterDynamic,
                                                        _data.MessageRandomizer.HandleCounter,
                                                        CliNodeValueType.Int
                                                        ),
                                            new CliNode(
                                                        "Generate Message",
                                                        _data.MessageRandomizer.GenerateAndSendRandomMessage
                                                        ),
                                            new CliNode(
                                                        "Counter Randomness",
                                                        _data.MessageRandomizer.GetRandomnessDynamic,
                                                        _data.MessageRandomizer.ToggleRandomness,
                                                        CliNodeValueType.State
                                                        ),
                                            new CliNode(
                                                        "Random Spreading",
                                                        _data.MessageRandomizer.Options.GetSpreadingDynamic,
                                                        _data.MessageRandomizer.Options.SetSpreadingDynamic,
                                                        CliNodeValueType.Range),
                                            new CliNode(
                                                        "Collect Logs",
                                                        _data.MessageRandomizer.GetLoggerStateDynamic,
                                                        _data.MessageRandomizer.ToggleLoggerState,
                                                        CliNodeValueType.State
                                                        ),
                                            new CliNode(
                                                        "State",
                                                        _data.MessageRandomizer.GetServiceStateDynamic,
                                                        _data.MessageRandomizer.ToggleService,
                                                        CliNodeValueType.State
                                                        )
                                        ]);

        var chatCmdsDir = new CliNode(
                                      "Chat Commands",
                                      DirectoryBack, 
                                      [
                                          new CliNode(
                                                   "Command Identifier",
                                                   _data.ChatCommands.Options.GetCommandIdentifierDynamic,
                                                   _data.ChatCommands.Options.SetCommandIdentifierDynamic,
                                                   CliNodeValueType.Char
                                                  ), 
                                          new CliNode(
                                                      "Service State",
                                                      _data.ChatCommands.GetServiceStateDynamic,
                                                      _data.ChatCommands.ToggleService,
                                                      CliNodeValueType.State
                                                      )
                                      ]);

        var patterns = new CliNode(
                                   "Patterns",
                                   "Add Pattern",
                                   "Remove Pattern",
                                   DirectoryBack, 
                                   CliNodeValueType.String
                                   );
        
        var regexDir = new CliNode(
                                   "Regex",
                                   DirectoryBack, 
                                   patterns.OuterNodes.ToArray(), 
                                   [
                                       patterns, 
                                       new CliNode(
                                                   "Service State",
                                                   _data.Regex.GetServiceStateDynamic,
                                                   _data.Regex.ToggleService,
                                                   CliNodeValueType.State
                                                   )
                                   ]);

        var services = new CliNode(
                                   "Services",
                                   DirectoryBack,
                                   [
                                       chatCmdsDir,
                                       gameReqsDir,
                                       randomMsgsDir,
                                       regexDir
                                   ]);
        
        var loginDir = new CliNode(
                                   "Credentials",
                                   DirectoryBack,
                                   [
                                       new CliNode(
                                                   "Load",
                                                   _data.Bot.Options.Load
                                                  ),
                                       new CliNode(
                                                   "Bot Username",
                                                   _data.Bot.Options.GetUsernameDynamic,
                                                   _data.Bot.Options.SetUsernameDynamic,
                                                   CliNodeValueType.String
                                                  ),
                                       new CliNode(
                                                   "Channel",
                                                   _data.Bot.Options.GetChannelDynamic,
                                                   _data.Bot.Options.SetChannelDynamic,
                                                   CliNodeValueType.String
                                                  ),
                                       new CliNode(
                                                   "OAuth",
                                                   _data.Bot.Options.GetOAuthDynamic,
                                                   _data.Bot.Options.SetOAuthDynamic,
                                                   CliNodeValueType.String
                                                  ),
                                       new CliNode(
                                                   "Save",
                                                   _data.Bot.Options.Save
                                                  )
                                   ]
                                         );

        var rootDir = new CliNode(
                                  "Root",
                                  null,
                                  [
                                      loginDir, 
                                      new CliNode(
                                                  "Initialize",
                                                  _data.Bot.Start
                                                           ),
                                      new CliNode(
                                                  "Enabled",
                                                  _data.Bot.GetServiceStateDynamic,
                                                  _data.Bot.ToggleService,
                                                  CliNodeValueType.State
                                                  ),
                                      services
                                  ]);

        _directories.Add(rootDir);
    }
}