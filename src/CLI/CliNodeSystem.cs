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
        var chatLogsDir = new CliNodeStaticDirectory(
                                                     ServiceName.ChatLogs,
                                                     _state,
                                                     true,
                                                     [
                                                         new CliNodeEnum(
                                                                         "ServiceState",
                                                                         _state.Data.ChatLogs.GetServiceStateAsInt,
                                                                         typeof(State),
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatLogs.ServiceStateNext
                                                                        ),
                                                     ]
                                                     );
        
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
                                                                             _state.Data.MessageRandomizer.GenerateAndSend,
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
                                                                            "Service State",
                                                                            _state.Data.MessageRandomizer.GetServiceStateAsInt,
                                                                            typeof(State),
                                                                            CliNodePermission.Default,
                                                                            _state.Data.MessageRandomizer.ServiceStateNext
                                                                            )
                                                       ]);
        
        var textGeneratorDir = new CliNodeStaticDirectory(
                                                          ServiceName.TextGenerator,
                                                          _state,
                                                          true,
                                                          [
                                                              new CliNodeAction(
                                                                                "Generate",
                                                                                _state.Data.TextGenerator.GenerateAndSend
                                                                                ),
                                                              new CliNodeStaticDirectory(
                                                                                         "Model",
                                                                                         _state,
                                                                                         true,
                                                                                         [
                                                                                             new CliNodeAction(
                                                                                                  "Train",
                                                                                                  _state.Data.TextGenerator.Train
                                                                                                  ),
                                                                                             new CliNodeInt(
                                                                                                  "Context Size",
                                                                                                  _state.Data.TextGenerator.Options.GetContextSize,
                                                                                                  CliNodePermission.Default,
                                                                                                  _state.Data.TextGenerator.Options.SetContextSize
                                                                                                 ),
                                                                                             new CliNodeInt(
                                                                                                  "Max Message Length",
                                                                                                  _state.Data.TextGenerator.Options.GetMaxLength,
                                                                                                  CliNodePermission.Default,
                                                                                                  _state.Data.TextGenerator.Options.SetMaxLength
                                                                                                 )
                                                                                         ]
                                                                                         ),
                                                              new CliNodeEnum(
                                                                              "Service State",
                                                                              _state.Data.TextGenerator.GetServiceStateAsInt,
                                                                              typeof(State),
                                                                              CliNodePermission.Default,
                                                                              _state.Data.TextGenerator.ServiceStateNext
                                                                              ),
                                                          ]
                                                          );

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
                                                          new CliNodeIndex(
                                                                         "Moderation Action Index",
                                                                         _state.Data.ChatCommands.GetModActionIndex,
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatCommands.SetModActionIndex
                                                                         ),
                                                          new CliNodeInt(
                                                                           "Cooldown",
                                                                           _state.Data.ChatCommands.GetCooldown,
                                                                           CliNodePermission.Default,
                                                                           _state.Data.ChatCommands.SetCooldown
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

        var levelReqsDir = new CliNodeStaticDirectory(
                                                      "Level Requests",
                                                      _state,
                                                      true,
                                                      [
                                                      new CliNodeIndex(
                                                                       "Moderation Pattern Index",
                                                                       _state.Data.LevelRequests.GetPatternIndex,
                                                                       CliNodePermission.Default,
                                                                       _state.Data.LevelRequests.SetPatternIndex
                                                                   ),
                                                      new CliNodeEnum(
                                                                      "Permission",
                                                                      _state.Data.LevelRequests.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      _state.Data.LevelRequests.RestrictionNext
                                                                     ),
                                                      new CliNodeEnum(
                                                                      "Service State",
                                                                      _state.Data.LevelRequests.GetServiceStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      _state.Data.LevelRequests.ServiceStateNext
                                                                     ),
                                                      ]
                                                     );

        var presetsDir = new CliNodeStaticDirectory(
                                                    "Presets",
                                                    _state,
                                                    true,
                                                    [
                                                    new CliNodeIndex(
                                                                     "Current Preset",
                                                                     _state.Data.Presets.Options.GetCurrentPreset,
                                                                     CliNodePermission.Default,
                                                                     _state.Data.Presets.SetCurrentPreset
                                                                     ),
                                                    new CliNodeDynamicDirectory(
                                                                                "Presets",
                                                                                "Add Preset",
                                                                                "Remove Preset",
                                                                                _state.Data.Presets.AddPreset,
                                                                                _state.Data.Presets.RemovePreset,
                                                                                _state.Data.Presets.GetPresetsAsContent(),
                                                                                _state,
                                                                                false
                                                                                ),
                                                    ]
                                                    );
        
        var servicesDir = new CliNodeStaticDirectory(
                                                  "Services",
                                                  _state,
                                                  true,
                                                  [
                                                      chatCmdsDir,
                                                      chatLogsDir,
                                                      randomMsgsDir,
                                                      textGeneratorDir,
                                                      messageFilterDir,
                                                      moderationDir,
                                                      levelReqsDir,
                                                      loggerDir,
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
                                                                                          "Broadcaster OAuth",
                                                                                          _state.Data.Bot.Options.GetBroadcasterOAuth,
                                                                                          CliNodePermission.Default,
                                                                                          _state.Data.Bot.Options.SetBroadcasterOAuth
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
                                                     presetsDir,
                                                     servicesDir
                                                 ]);

        _directories.Add(rootDir);
    }
}