using ChatBot.CLI.CliNodes;
using ChatBot.shared.interfaces;

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
        var gameReqsDir = new CliNodeGeneric<int>(
                                                      "Game Requests",
                                                      [
                                                          new CliNodeGeneric<State>(
                                                                             "State",
                                                                             _data.GameRequests.GetServiceState,
                                                                             _data.GameRequests.ToggleService,
                                                                             CliNodeValueType.State
                                                                            )
                                                      ],
                                                      DirectoryBack
                                                     );

        var randomMsgsDir = new CliNodeGeneric<int>(
                                                        "Random Messages",
                                                        [
                                                            new CliNodeGeneric<int>(
                                                                                    "Max Counter Value",
                                                                                    _data.MessageRandomizer.Options.GetCounterMax,
                                                                                    _data.MessageRandomizer.Options.SetCounterMax,
                                                                                    CliNodeValueType.Int
                                                                                   ),
                                                            new CliNodeGeneric<int>(
                                                                                    "Random Value",
                                                                                    _data.MessageRandomizer.Options.GetRandomValue,
                                                                                    CliNodeValueType.Int
                                                                                   ),
                                                            new CliNodeGeneric<int>(
                                                                               "Increase Counter",
                                                                               _data.MessageRandomizer.GetCounter,
                                                                               _data.MessageRandomizer.HandleCounter,
                                                                               CliNodeValueType.Int
                                                                              ),
                                                            new CliNodeGeneric<int>(
                                                                               "Generate Message",
                                                                               _data.MessageRandomizer.GenerateAndSendRandomMessage
                                                                              ),
                                                            new CliNodeGeneric<State>(
                                                                                      "Counter Randomness",
                                                                                      _data.MessageRandomizer.GetRandomness,
                                                                                      _data.MessageRandomizer.ToggleRandomness,
                                                                                      CliNodeValueType.State
                                                                                     ),
                                                            new CliNodeGeneric<Range>(
                                                                                      "Random Spreading",
                                                                                      _data.MessageRandomizer.Options.GetSpreading,
                                                                                      _data.MessageRandomizer.Options.SetSpreading,
                                                                                      CliNodeValueType.Range
                                                                                     ),
                                                            new CliNodeGeneric<State>(
                                                                               "Collect Logs",
                                                                               _data.MessageRandomizer.GetLoggerState,
                                                                               _data.MessageRandomizer.ToggleLoggerState,
                                                                               CliNodeValueType.State
                                                                              ),
                                                            new CliNodeGeneric<State>(
                                                                               "State",
                                                                               _data.MessageRandomizer.GetServiceState,
                                                                               _data.MessageRandomizer.ToggleService,
                                                                               CliNodeValueType.State
                                                                              )
                                                        ],
                                                        DirectoryBack
                                                       );

        var chatCmdsDir = new CliNodeGeneric<int>(
                                                      "Chat Commands",
                                                      [
                                                          new CliNodeGeneric<char>(
                                                                                   "Command Identifier",
                                                                                   _data.ChatCommands.Options.GetCommandIdentifier,
                                                                                   _data.ChatCommands.Options.SetCommandIdentifier,
                                                                                   CliNodeValueType.Char
                                                                                  ),
                                                          new CliNodeGeneric<State>(
                                                                             "Service State",
                                                                             _data.ChatCommands.GetServiceState,
                                                                             _data.ChatCommands.ToggleService,
                                                                             CliNodeValueType.State
                                                                            )
                                                      ],
                                                      DirectoryBack
                                                     );

        var regexDir = new CliNodeGeneric<int>(
                                                   "Regex",
                                                   [
                                                       new CliNodeGeneric<int>(
                                                                          "Patterns",
                                                                          [
                                                                              new CliNodeGeneric<string>(
                                                                                   "Add Pattern",
                                                                                   _data.Regex.AddPattern,
                                                                                   CliNodeValueType.String
                                                                                  )
                                                                          ],
                                                                          DirectoryBack
                                                                         ),
                                                       new CliNodeGeneric<State>(
                                                                               "Service State",
                                                                               _data.Regex.GetServiceState,
                                                                               _data.Regex.ToggleService,
                                                                               CliNodeValueType.State
                                                                              )
                                                   ],
                                                   DirectoryBack
                                                  );

        var services = new CliNodeGeneric<int>(
                                                   "Services",
                                                   [
                                                       chatCmdsDir,
                                                       gameReqsDir,
                                                       randomMsgsDir,
                                                       regexDir
                                                   ],
                                                   DirectoryBack
                                                  );


        var loginDir = new CliNodeGeneric<int>(
                                                   "Credentials",
                                                   [
                                                       new CliNodeGeneric<int>(
                                                                            "Load",
                                                                            _data.Bot.Options.Load
                                                                           ),
                                                       new CliNodeGeneric<string>(
                                                                                  "Bot Username",
                                                                                  _data.Bot.Options.GetUsername,
                                                                                  _data.Bot.Options.SetUsername,
                                                                                  CliNodeValueType.String
                                                                                 ),
                                                       new CliNodeGeneric<string>(
                                                                                  "Channel",
                                                                                  _data.Bot.Options.GetChannel,
                                                                                  _data.Bot.Options.SetChannel,
                                                                                  CliNodeValueType.String
                                                                                 ),
                                                       new CliNodeGeneric<string>(
                                                                                  "OAuth",
                                                                                  _data.Bot.Options.GetOAuth,
                                                                                  _data.Bot.Options.SetOAuth,
                                                                                  CliNodeValueType.String
                                                                                 ),
                                                       new CliNodeGeneric<int>(
                                                                               "Save",
                                                                               _data.Bot.Options.Save
                                                                              )
                                                   ],
                                                   DirectoryBack
                                                  );

        var rootDir = new CliNodeGeneric<int>(
                                           "Root",
                                           [
                                               loginDir,
                                               new CliNodeGeneric<int>(
                                                                       "Initialize",
                                                                       _data.Bot.Start
                                                                      ),
                                               new CliNodeGeneric<State>(
                                                                         "Enabled",
                                                                         _data.Bot.GetServiceState,
                                                                         _data.Bot.ToggleService,
                                                                         CliNodeValueType.State
                                                                        ),
                                               services
                                           ]
                                          );

        _directories.Add(rootDir);
    }
}