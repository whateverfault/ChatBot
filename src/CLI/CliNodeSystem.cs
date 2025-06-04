using ChatBot.CLI.CliNodes;
using ChatBot.CLI.CliNodes.Client;
using ChatBot.CLI.CliNodes.Directories;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

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
                                                           new CliNodeEnum(
                                                                           "Counter Randomness",
                                                                           _state.Data.MessageRandomizer.GetRandomnessAsInt,
                                                                           typeof(State),
                                                                           CliNodePermission.Default,
                                                                           _state.Data.MessageRandomizer.RandomnessNext
                                                                          ),
                                                           new CliNodeRange(
                                                                          "Random Spreading",
                                                                          _state.Data.MessageRandomizer.Options.GetSpreading,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.MessageRandomizer.Options.SetSpreading
                                                                          ),
                                                           new CliNodeEnum(
                                                                           "Collect Logs",
                                                                           _state.Data.MessageRandomizer.GetLoggerStateAsInt,
                                                                           typeof(State),
                                                                           CliNodePermission.Default,
                                                                           _state.Data.MessageRandomizer.LoggerStateNext
                                                                          ),
                                                           new CliNodeEnum(
                                                                            "Service State",
                                                                            _state.Data.MessageRandomizer.GetServiceStateAsInt,
                                                                            typeof(State),
                                                                            CliNodePermission.Default,
                                                                            _state.Data.MessageRandomizer.ServiceStateNext
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
                                                          new CliNodeEnum(
                                                                          "Required Role",
                                                                          _state.Data.ChatCommands.GetRequiredRoleAsInt,
                                                                          typeof(Restriction),
                                                                          CliNodePermission.Default,
                                                                          _state.Data.ChatCommands.RequiredRoleNext
                                                                          ),
                                                          new CliNodeInt(
                                                                         "Moderation Action Index",
                                                                         _state.Data.ChatCommands.GetModActionIndex,
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatCommands.SetModActionIndex
                                                                         ),
                                                          new CliNodeEnum(
                                                                         "Service State",
                                                                         _state.Data.ChatCommands.GetServiceStateAsInt,
                                                                         typeof(State),
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatCommands.ServiceStateNext
                                                                         ),
                                                      ]);

        var timeoutDir = new CliNodeDynamicModerationDirectory(
                                                               "Timeout Patterns",
                                                               "Add Pattern",
                                                               "Remove Pattern",
                                                               ModerationActionType.Timeout,
                                                               _state
                                                              );
        
        var banDir = new CliNodeDynamicModerationDirectory(
                                                               "Ban Patterns",
                                                               "Add Pattern",
                                                               "Remove Pattern",
                                                               ModerationActionType.Ban,
                                                               _state
                                                              );
        
        var warnDir = new CliNodeDynamicModerationDirectory(
                                                           "Warn Patterns",
                                                           "Add Pattern",
                                                           "Remove Pattern",
                                                           ModerationActionType.Warn,
                                                           _state
                                                          );
        
        var warnWithTimeoutDir = new CliNodeDynamicModerationDirectory(
                                                            "Warn With Timeout Patterns",
                                                            "Add Pattern",
                                                            "Remove Pattern",
                                                            ModerationActionType.WarnWithTimeout,
                                                            _state
                                                           );
        
        var warnWithBanDir = new CliNodeDynamicModerationDirectory(
                                                            "Warn With Ban Patterns",
                                                            "Add Pattern",
                                                            "Remove Pattern",
                                                            ModerationActionType.WarnWithBan,
                                                            _state
                                                           );
        
        var moderationDir = new CliNodeStaticDirectory(
                                                       ServiceName.Moderation,
                                                       _state,
                                                       true,
                                                       [
                                                           banDir,
                                                           timeoutDir,
                                                           warnWithBanDir,
                                                           warnWithTimeoutDir,
                                                           warnDir,
                                                           new CliNodeEnum(
                                                                           "Service State",
                                                                           _state.Data.Moderation.GetServiceStateAsInt,
                                                                           typeof(State),
                                                                           CliNodePermission.Default,
                                                                           _state.Data.Moderation.ServiceStateNext
                                                                          ),
                                                       ]
                                                      );
        
        var globalPatterns = new CliNodeDynamicDirectory(
                                                   "Global Patterns",
                                                   "Add Pattern",
                                                   "Remove Pattern",
                                                   _state.Data.MessageFilter.AddPatternWithComment,
                                                   _state.Data.MessageFilter.RemovePattern,
                                                   _state.Data.MessageFilter.GetPatternsWithComments(),
                                                   _state,
                                                   true
                                                   );
        
        var messageFilterDir = new CliNodeStaticDirectory(
                                                  ServiceName.MessageFilter,
                                                  _state,
                                                  true,
                                                  [
                                                      globalPatterns,
                                                      moderationDir,
                                                      new CliNodeEnum(
                                                                      "Service State",
                                                                      _state.Data.MessageFilter.GetServiceStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      _state.Data.MessageFilter.ServiceStateNext
                                                                     ),
                                                  ]);

        var loggerDir = new CliNodeStaticDirectory(
                                                   ServiceName.Logger,
                                                   _state,
                                                   true,
                                                   [
                                                       new CliNodeLogDirectory(
                                                                               "Non-Twitch Logs",
                                                                               true,
                                                                               _state,
                                                                               _state.Data.Logger,
                                                                               LogType.NonTwitch
                                                                               ),
                                                       new CliNodeLogDirectory(
                                                                               "Twitch Logs",
                                                                               true,
                                                                               _state,
                                                                               _state.Data.Logger,
                                                                               LogType.Twitch
                                                                              ),
                                                       new CliNodeEnum(
                                                                       "Service State",
                                                                       _state.Data.Logger.GetServiceStateAsInt,
                                                                       typeof(State),
                                                                       CliNodePermission.Default,
                                                                       _state.Data.Logger.ServiceStateNext
                                                                      ),
                                                   ]
                                                   ); 
        
        var services = new CliNodeStaticDirectory(
                                                  "Services",
                                                  _state,
                                                  true,
                                                  [
                                                      chatCmdsDir,
                                                      randomMsgsDir,
                                                      messageFilterDir,
                                                      loggerDir
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
                                                      new CliNodeStaticDirectory(
                                                                                 "Secret",
                                                                                 _state,
                                                                                 true,
                                                                                 [
                                                                                     new CliNodeString(
                                                                                          "OAuth",
                                                                                          _state.Data.Bot.Options.GetOAuth,
                                                                                          CliNodePermission.Default,
                                                                                          _state.Data.Bot.Options.SetOAuth
                                                                                         ),
                                                                                     new CliNodeString(
                                                                                          "ClientId",
                                                                                          _state.Data.Bot.Options.GetClientId,
                                                                                          CliNodePermission.Default,
                                                                                          _state.Data.Bot.Options.SetClientId
                                                                                         ),
                                                                                 ]
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
                                                     services
                                                 ]);

        _directories.Add(rootDir);
    }
}