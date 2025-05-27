using ChatBot.CLI.CliNodes;
using ChatBot.CLI.CliNodes.Client;
using ChatBot.CLI.CliNodes.Directories;
using ChatBot.Services.Static;

namespace ChatBot.CLI;

public class CliNodeSystem {
    private readonly CliState _state;
    private readonly List<CliNodeDirectory> _directories;
    private int _dirPointer;


    public CliNodeSystem(CliState state) {
        _directories = [];
        _state = state;
    }

    public CliNodeDirectory Current => _directories[_dirPointer];
    
    
    public void DirectoryEnter(CliNodeDirectory dir) {
        _directories.Add(dir);
        _dirPointer++;
    }
    
    public void DirectoryBack() {
        if (_dirPointer > 0) {
            _directories.RemoveAt(_dirPointer--);
        }
    }

    public void InitNodes() {
        var gameReqsDir = new CliNodeStaticDirectory(
                                                     ServiceName.GameRequests,
                                                     _state, 
                                                     true,
                                                     [
                                                         new CliNodeState(
                                                                          "Service State",
                                                                          _state.Data.GameRequests.GetServiceState,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.GameRequests.ToggleService
                                                                          )
                                                     ]);

        var randomMsgsDir = new CliNodeStaticDirectory(
                                                       ServiceName.MessageRandomizer,
                                                       _state,
                                                       true,
                                                       [
                                                           new CliNodeInt(
                                                                          "Max Counter Value",
                                                                          _state.Data.MessageRandomizer.Options.GetCounterMax,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.MessageRandomizer.Options.SetCounterMax
                                                                          ),
                                                           new CliNodeInt(
                                                                          "Random Value", 
                                                                          _state.Data.MessageRandomizer.Options.GetRandomValue,
                                                                          CliNodePermission.ReadOnly
                                                                          ),
                                                           new CliNodeClientWithInt(
                                                                                    "Increase Counter",
                                                                                    _state.Data.MessageRandomizer.HandleCounter,
                                                                                    _state.Data.MessageRandomizer.GetCounter
                                                                                    ),
                                                           new CliNodeClient(
                                                                             "Generate Message",
                                                                             _state.Data.MessageRandomizer.GenerateAndSendRandomMessage,
                                                                             _state
                                                                            ),
                                                           new CliNodeState(
                                                                          "Counter Randomness",
                                                                          _state.Data.MessageRandomizer.GetRandomness,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.MessageRandomizer.ToggleRandomness
                                                                          ),
                                                           new CliNodeRange(
                                                                          "Random Spreading",
                                                                          _state.Data.MessageRandomizer.Options.GetSpreading,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.MessageRandomizer.Options.SetSpreading
                                                                          ),
                                                           new CliNodeState(
                                                                            "Collect Logs",
                                                                            _state.Data.MessageRandomizer.GetLoggerState,
                                                                            CliNodePermission.Default,
                                                                            _state.Data.MessageRandomizer.ToggleLoggerState
                                                                            ),
                                                           new CliNodeState(
                                                                            "Service State",
                                                                            _state.Data.MessageRandomizer.GetServiceState,
                                                                            CliNodePermission.Default,
                                                                            _state.Data.MessageRandomizer.ToggleService
                                                                            )
                                                       ]);

        var chatCmdsDir = new CliNodeStaticDirectory(
                                                     ServiceName.ChatCommands,
                                                      _state, 
                                                      true,
                                                      [
                                                          new CliNodeChar(
                                                                          "Command Identifier",
                                                                          _state.Data.ChatCommands.Options.GetCommandIdentifier,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.ChatCommands.SetCommandIdentifier
                                                                          ), 
                                                          new CliNodeState(
                                                                         "Service State",
                                                                         _state.Data.ChatCommands.GetServiceState,
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatCommands.ToggleService
                                                                         ),
                                                      ]);

        var patterns = new CliNodeDynamicDirectory(
                                                   "Global Patterns",
                                                   "Add Pattern",
                                                   "Remove Pattern",
                                                   _state.Data.MessageFilter.AddPatternWithComment,
                                                   _state.Data.MessageFilter.RemovePattern,
                                                   _state.Data.MessageFilter.GetPatternWithComments(),
                                                   _state,
                                                   true
                                                   );
        
        var messageFilterDir = new CliNodeStaticDirectory(
                                                  ServiceName.MessageFilter,
                                                  _state,
                                                  true,
                                                  [
                                                      patterns,
                                                      new CliNodeState(
                                                                       "Service State",
                                                                       _state.Data.MessageFilter.GetServiceState,
                                                                       CliNodePermission.Default,
                                                                       _state.Data.MessageFilter.ToggleService
                                                                       ),
                                                  ]);

        var services = new CliNodeStaticDirectory(
                                                  "Services",
                                                  _state,
                                                  true,
                                                  [
                                                      chatCmdsDir,
                                                      gameReqsDir,
                                                      randomMsgsDir,
                                                      messageFilterDir
                                                  ]);
        
        var loginDir = new CliNodeStaticDirectory(
                                                  "Credentials",
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeString(
                                                                        "Bot Username",
                                                                        _state.Data.Bot.Options.GetUsername,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.Bot.Options.SetUsername
                                                                        ),
                                                      new CliNodeString(
                                                                        "Channel",
                                                                        _state.Data.Bot.Options.GetChannel,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.Bot.Options.SetChannel
                                                                       ),
                                                      new CliNodeString(
                                                                        "OAuth",
                                                                        _state.Data.Bot.Options.GetOAuth,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.Bot.Options.SetOAuth
                                                                        ),
                                                      new CliNodeAction(
                                                                        "Load",
                                                                        _state.Data.Bot.Options.Load
                                                                       ),
                                                      new CliNodeAction(
                                                                        "Save",
                                                                        _state.Data.Bot.Options.Save
                                                                       )
                                                  ]
                                                 );

        var rootDir = new CliNodeStaticDirectory(
                                                 "Root",
                                                 _state,
                                                 false,
                                                 [
                                                     loginDir, 
                                                     new CliNodeAction(
                                                                       "Initialize",
                                                                       _state.Data.Bot.Start
                                                                      ),
                                                     new CliNodeState(
                                                                      "Enabled",
                                                                      _state.Data.Bot.GetServiceState,
                                                                      CliNodePermission.Default,
                                                                      _state.Data.Bot.ToggleService
                                                                      ),
                                                     services
                                                 ]);

        _directories.Add(rootDir);
    }
}