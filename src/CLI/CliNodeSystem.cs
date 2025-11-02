using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.translator;
using ChatBot.bot.shared.handlers;
using ChatBot.cli.CliNodes;
using ChatBot.cli.CliNodes.Directories;
using ChatBot.cli.CliNodes.Directories.ChatAds;
using ChatBot.cli.CliNodes.Directories.ChatCommands;
using ChatBot.cli.CliNodes.Directories.MessageFilter;
using ChatBot.cli.CliNodes.Directories.Moderation;
using ChatBot.cli.CliNodes.Directories.Presets;
using TwitchAPI.client;

namespace ChatBot.cli;

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
                                                                         "Service State",
                                                                         _state.Data.ChatLogs.GetServiceStateAsInt,
                                                                         typeof(State),
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatLogs.ServiceStateNext
                                                                        ),
                                                         new CliNodeInt(
                                                                         "Message Count",
                                                                         _state.Data.ChatLogs.GetLogsCount,
                                                                         CliNodePermission.ReadOnly
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
                                                           new CliNodeCounter(
                                                                              "Increase Counter",
                                                                              _state.Data.MessageRandomizer.GetCounter,
                                                                              _state.Data.MessageRandomizer.HandleCounter
                                                                             ),
                                                           new CliNodeAction(
                                                                             "Generate Message",
                                                                             _state.Data.MessageRandomizer.GenerateAndSend
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
                                                                            ),
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
                                                                                                 ),
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

        var chatAdsDir = new CliNodeStaticDirectory(
                                                    ServiceName.ChatAds,
                                                    _state,
                                                    true,
                                                    [
                                                        new CliNodeDynamicChatAdsDirectory(
                                                             "Ads",
                                                             "Add Chat Ad",
                                                             "Remove Chat Ad",
                                                             _state
                                                            ),
                                                        new CliNodeEnum(
                                                                        "Service State",
                                                                        _state.Data.ChatAds.GetServiceStateAsInt,
                                                                        typeof(State),
                                                                        CliNodePermission.Default,
                                                                        _state.Data.ChatAds.ServiceStateNext
                                                                       ),
                                                    ]
                                                    );
        
        var defaultCmds = new CliNodeStaticDirectory(
                                                     "Default Commands",
                                                     _state,
                                                     true,
                                                     []
                                                     );
        foreach (var cmd in _state.Data.ChatCommands.Options.DefaultCmds) {
            if (string.IsNullOrEmpty(cmd.Name)) continue;
            defaultCmds.AddNode(
                                new CliNodeStaticDirectory(
                                              cmd.Name,
                                              _state,
                                              true,
                                              [
                                                  new CliNodeString(
                                                                    "Name",
                                                                    cmd.GetName,
                                                                    CliNodePermission.Default,
                                                                    cmd.SetName
                                                                    ),
                                                  new CliNodeString(
                                                                    "Arguments",
                                                                    cmd.GetArgs,
                                                                    CliNodePermission.Default,
                                                                    cmd.SetArgs
                                                                   ),
                                                  new CliNodeAliases(
                                                                     "Aliases",
                                                                     cmd.GetAliases,
                                                                     CliNodePermission.Default,
                                                                     cmd.SetAliases
                                                                    ),
                                                  new CliNodeString(
                                                                    "Description",
                                                                    cmd.GetDescription,
                                                                    CliNodePermission.Default,
                                                                    cmd.SetDescription
                                                                   ),
                                                  new CliNodeTime(
                                                                 "Cooldown",
                                                                 cmd.GetCooldown,
                                                                 CliNodePermission.Default,
                                                                 cmd.SetCooldown
                                                                 ),
                                                  new CliNodeBool(
                                                                  "Cooldown Per User",
                                                                  cmd.GetCooldownPerUser,
                                                                  CliNodePermission.Default,
                                                                  cmd.SetCooldownPerUser
                                                                  ),
                                                  new CliNodeEnum(
                                                                  "Permission",
                                                                  cmd.GetRestrictionAsInt,
                                                                  typeof(Restriction),
                                                                  CliNodePermission.Default,
                                                                  cmd.RestrictionNext
                                                                  ),
                                                  new CliNodeEnum(
                                                                  "State",
                                                                  cmd.GetStateAsInt,
                                                                  typeof(State),
                                                                  CliNodePermission.Default,
                                                                  cmd.StateNext
                                                                 ),
                                              ])
                                );
        }
        
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
                                                          defaultCmds,
                                                          new CliNodeDynamicChatCmdsDirectory(
                                                                                              "Custom Commands",
                                                                                              "Add Custom Command",
                                                                                              "Remove Custom Command",
                                                                                              _state
                                                                                              ),
                                                          chatAdsDir,
                                                          new CliNodeStaticDirectory(
                                                                                     "Special",
                                                                                     _state,
                                                                                     true,
                                                                                     [
                                                                                        new CliNodeString(
                                                                                             "Base Title",
                                                                                             _state.Data.ChatCommands.GetBaseTitle,
                                                                                             CliNodePermission.Default,
                                                                                             _state.Data.ChatCommands.SetBaseTitle
                                                                                             ),
                                                                                        new CliNodeEnum(
                                                                                             "Send Whisper If Possible",
                                                                                             _state.Data.ChatCommands.GetSendWhisperIfPossibleStateAsInt,
                                                                                             typeof(State),
                                                                                             CliNodePermission.Default,
                                                                                             _state.Data.ChatCommands.SendWhisperIfPossibleStateNext
                                                                                            ),
                                                                                     ]
                                                                                     ),
                                                          new CliNodeEnum(
                                                                          "Verbose State",
                                                                          _state.Data.ChatCommands.GetVerboseStateAsInt,
                                                                          typeof(State),
                                                                          CliNodePermission.Default,
                                                                          _state.Data.ChatCommands.VerboseStateNext
                                                                         ),
                                                          new CliNodeEnum(
                                                                         "Service State",
                                                                         _state.Data.ChatCommands.GetServiceStateAsInt,
                                                                         typeof(State),
                                                                         CliNodePermission.Default,
                                                                         _state.Data.ChatCommands.ServiceStateNext
                                                                         ),
                                                      ]);
        
        var moderationDir = new CliNodeStaticDirectory(
                                                       ServiceName.Moderation,
                                                       _state,
                                                       true,
                                                       [
                                                           new CliNodeDynamicModerationDirectory(
                                                                                                 "Moderation Patterns",
                                                                                                 "Add Pattern",
                                                                                                 "Remove Pattern",
                                                                                                 _state
                                                                                                ),
                                                           new CliNodeEnum(
                                                                           "Service State",
                                                                           _state.Data.Moderation.GetServiceStateAsInt,
                                                                           typeof(State),
                                                                           CliNodePermission.Default,
                                                                           _state.Data.Moderation.ServiceStateNext
                                                                          ),
                                                       ]
                                                      );
        
        var globalPatterns = new CliNodeMessageFilterDynamicDirectory(
                                                                      "Global Filters",
                                                                      "Add Filter",
                                                                      "Remove Filter",
                                                                      _state
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
                                                                               "Logs",
                                                                               true,
                                                                               _state,
                                                                               _state.Data.Logger
                                                                               ),
                                                       new CliNodeEnum(
                                                                       "Log Level",
                                                                       _state.Data.Logger.GetLogLevelAsInt,
                                                                       typeof(LogLevel),
                                                                       CliNodePermission.Default,
                                                                       _state.Data.Logger.LogLevelNext
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
                                                      ServiceName.LevelRequests,
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
                                                                      "Requests State",
                                                                      _state.Data.LevelRequests.GetReqStateAsInt,
                                                                      typeof(ReqState),
                                                                      CliNodePermission.Default,
                                                                      _state.Data.LevelRequests.ReqStateNext
                                                                      ),
                                                      new CliNodeString(
                                                                        "Reward Id",
                                                                        _state.Data.LevelRequests.GetRewardId,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.LevelRequests.SetRewardId
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

        var demonListDir = new CliNodeStaticDirectory(
                                                      ServiceName.DemonList,
                                                      _state,
                                                      true,
                                                      [
                                                      new CliNodeEnum(
                                                                      "Service State",
                                                                      _state.Data.DemonList.GetServiceStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      _state.Data.DemonList.ServiceStateNext
                                                                      ),
                                                      ]
                                                      );

        var aiDir = new CliNodeStaticDirectory(
                                               ServiceName.Ai,
                                               _state,
                                               true,
                                               [
                                                   new CliNodeEnum(
                                                                   "AI Mode",
                                                                   _state.Data.Ai.GetAiKindAsInt,
                                                                   typeof(AiKind),
                                                                   CliNodePermission.Default,
                                                                   _state.Data.Ai.AiKindNext
                                                                   ),
                                                   new CliNodeStaticDirectory(
                                                                              "Ollama",
                                                                              _state,
                                                                              true,
                                                                              [
                                                                                  new CliNodeString(
                                                                                       "Model",
                                                                                       _state.Data.Ai.GetOllamaModel,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetOllamaModel
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Prompt",
                                                                                       _state.Data.Ai.GetOllamaPrompt,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetOllamaPrompt
                                                                                      ),
                                                                              ]
                                                                             ),
                                                   new CliNodeStaticDirectory(
                                                                              "HuggingFace",
                                                                              _state,
                                                                              true,
                                                                              [
                                                                                  new CliNodeString(
                                                                                       "Model",
                                                                                       _state.Data.Ai.GetHfModel,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetHfModel
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Provider",
                                                                                       _state.Data.Ai.GetHfProvider,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetHfProvider
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Prompt",
                                                                                       _state.Data.Ai.GetHfPrompt,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetHfPrompt
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback State",
                                                                                       _state.Data.Ai.GetHfFallbackStateAsInt,
                                                                                       typeof(State),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.HfFallbackStateNext
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback Ai",
                                                                                       _state.Data.Ai.GetHfFallbackAiAsInt,
                                                                                       typeof(AiKind),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.HfFallbackAiNext
                                                                                      ),
                                                                                  new CliNodeStaticDirectory(
                                                                                       "Secret",
                                                                                       _state,
                                                                                       true,
                                                                                       [
                                                                                           new CliNodeString(
                                                                                                "Api Token",
                                                                                                _state.Data.Ai.GetHfToken,
                                                                                                CliNodePermission.Default,
                                                                                                _state.Data.Ai.SetHfToken
                                                                                               ),
                                                                                           ]
                                                                                       ),
                                                                              ]
                                                                             ),
                                                   new CliNodeStaticDirectory(
                                                                              "Vertex AI",
                                                                              _state,
                                                                              true,
                                                                              [
                                                                                  new CliNodeString(
                                                                                       "Model",
                                                                                       _state.Data.Ai.GetVertexModel,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetVertexModel
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Project Id",
                                                                                       _state.Data.Ai.GetGoogleProjectId,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetGoogleProjectId
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Prompt",
                                                                                       _state.Data.Ai.GetVertexPrompt,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetVertexPrompt
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback State",
                                                                                       _state.Data.Ai.GetVertexFallbackStateAsInt,
                                                                                       typeof(State),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.VertexFallbackStateNext
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback Ai",
                                                                                       _state.Data.Ai.GetVertexFallbackAiAsInt,
                                                                                       typeof(AiKind),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.VertexFallbackAiNext
                                                                                      ),
                                                                                  new CliNodeStaticDirectory(
                                                                                       "Secret",
                                                                                       _state,
                                                                                       true,
                                                                                       [
                                                                                           new CliNodeString(
                                                                                                "Api Token",
                                                                                                _state.Data.Ai.GetVertexToken,
                                                                                                CliNodePermission.Default,
                                                                                                _state.Data.Ai.SetVertexToken
                                                                                               ),
                                                                                           ]
                                                                                       ),
                                                                              ]
                                                                             ),
                                                                                                      new CliNodeStaticDirectory(
                                                                              "DeepSeek",
                                                                              _state,
                                                                              true,
                                                                              [
                                                                                  new CliNodeString(
                                                                                       "Model",
                                                                                       _state.Data.Ai.GetDeepSeekModel,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetDeepSeekModel
                                                                                      ),
                                                                                  new CliNodeString(
                                                                                       "Prompt",
                                                                                       _state.Data.Ai.GetDeepSeekPrompt,
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.SetDeepSeekPrompt
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback State",
                                                                                       _state.Data.Ai.GetDeepSeekFallbackStateAsInt,
                                                                                       typeof(State),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.DeepSeekFallbackStateNext
                                                                                      ),
                                                                                  new CliNodeEnum(
                                                                                       "Fallback Ai",
                                                                                       _state.Data.Ai.GetDeepSeekFallbackAiAsInt,
                                                                                       typeof(AiKind),
                                                                                       CliNodePermission.Default,
                                                                                       _state.Data.Ai.DeepSeekFallbackAiNext
                                                                                      ),
                                                                                  new CliNodeStaticDirectory(
                                                                                       "Secret",
                                                                                       _state,
                                                                                       true,
                                                                                       [
                                                                                           new CliNodeString(
                                                                                                "Api Token",
                                                                                                _state.Data.Ai.GetDeepSeekToken,
                                                                                                CliNodePermission.Default,
                                                                                                _state.Data.Ai.SetDeepSeekToken
                                                                                               ),
                                                                                           ]
                                                                                       ),
                                                                              ]
                                                                             ),
                                                   new CliNodeTime(
                                                                   "Remove Chat In",
                                                                   _state.Data.Ai.GetRemoveChatIn,
                                                                   CliNodePermission.Default,
                                                                   _state.Data.Ai.SetRemoveChatIn,
                                                                   isUnixEpoch: false
                                                                  ),
                                                   new CliNodeEnum(
                                                                   "Service State",
                                                                   _state.Data.Ai.GetServiceStateAsInt,
                                                                   typeof(State),
                                                                   CliNodePermission.Default,
                                                                   _state.Data.Ai.ServiceStateNext
                                                                  ),
                                               ]
                                               );

        var translatorDir = new CliNodeStaticDirectory(
                                                       ServiceName.Translator,
                                                       _state,
                                                       true,
                                                       [
                                                           new CliNodeEnum(
                                                                             "Translation Service",
                                                                             _state.Data.Translator.GetTranslationServiceAsInt,
                                                                             typeof(TranslationService),
                                                                             CliNodePermission.Default,
                                                                             _state.Data.Translator.TranslationServiceNext
                                                                            ),
                                                           new CliNodeString(
                                                                             "Target Language",
                                                                             _state.Data.Translator.GetTargetLanguage,
                                                                             CliNodePermission.Default,
                                                                             _state.Data.Translator.SetTargetLanguage
                                                                            ),
                                                           new CliNodeStaticDirectory(
                                                                                      "Google Translator",
                                                                                      _state,
                                                                                      true,
                                                                                      [
                                                                                          new CliNodeString(
                                                                                               "Project Id",
                                                                                               _state.Data.Translator.GetProjectId,
                                                                                               CliNodePermission.Default,
                                                                                               _state.Data.Translator.SetProjectId
                                                                                               ),
                                                                                          new CliNodeString(
                                                                                               "Location",
                                                                                               _state.Data.Translator.GetLocation,
                                                                                               CliNodePermission.Default,
                                                                                               _state.Data.Translator.SetLocation
                                                                                              ),
                                                                                          new CliNodeStaticDirectory(
                                                                                               "Secret",
                                                                                               _state,
                                                                                               true,
                                                                                               [
                                                                                                   new CliNodeString(
                                                                                                        "Api Token",
                                                                                                        _state.Data.Translator.GetGoogleToken,
                                                                                                        CliNodePermission.Default,
                                                                                                        _state.Data.Translator.SetGoogleToken
                                                                                                        ),
                                                                                               ]
                                                                                               ),
                                                                                      ]
                                                                                      ),
                                                           new CliNodeStaticDirectory(
                                                                                      "Vk Translator",
                                                                                      _state,
                                                                                      true,
                                                                                      [
                                                                                          new CliNodeStaticDirectory(
                                                                                               "Secret",
                                                                                               _state,
                                                                                               true,
                                                                                               [
                                                                                                   new CliNodeString(
                                                                                                        "Api Token",
                                                                                                        _state.Data.Translator.GetVkToken,
                                                                                                        CliNodePermission.Default,
                                                                                                        _state.Data.Translator.SetVkToken
                                                                                                        ),
                                                                                               ]
                                                                                               ),
                                                                                      ]
                                                                                      ),
                                                           new CliNodeEnum(
                                                                           "Service State",
                                                                           _state.Data.Translator.GetServiceStateAsInt,
                                                                           typeof(State),
                                                                           CliNodePermission.Default,
                                                                           _state.Data.Translator.ServiceStateNext
                                                                           ),
                                                       ]
                                                       );

        var gameReqsDir = new CliNodeStaticDirectory(
                                                     ServiceName.GameRequests,
                                                     _state,
                                                     true,
                                                     [
                                                         new CliNodeEnum(
                                                                         "Service State",
                                                                         _state.Data.GameRequests.GetServiceStateAsInt,
                                                                         typeof(State),
                                                                         CliNodePermission.Default,
                                                                         _state.Data.GameRequests.ServiceStateNext
                                                                        ),
                                                     ]
                                                     );
        
        var streamStateCheckerDir = new CliNodeStaticDirectory(
                                                               ServiceName.StreamStateChecker,
                                                               _state,
                                                               true,
                                                               [
                                                                   new CliNodeTime(
                                                                        "Check Cooldown",
                                                                        _state.Data.StreamStateChecker.Options.GetCheckCooldown,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.StreamStateChecker.Options.SetCheckCooldown,
                                                                        isUnixEpoch: false
                                                                       ),
                                                                   new CliNodeStaticDirectory(
                                                                        "Info For Nerds",
                                                                        _state,
                                                                        true,
                                                                        [
                                                                            new CliNodeTime(
                                                                                 "Last Time Checked", 
                                                                                 _state.Data.StreamStateChecker.Options.GetLastCheckTime,
                                                                                 CliNodePermission.ReadOnly,
                                                                                 isUnixEpoch: true
                                                                                ),
                                                                            new CliNodeTime(
                                                                                "Last Time Was Online",
                                                                                _state.Data.StreamStateChecker.Options.GetLastOnline,
                                                                                CliNodePermission.ReadOnly,
                                                                                isUnixEpoch: true
                                                                               ),
                                                                           new CliNodeTime(
                                                                                "Online Time",
                                                                                _state.Data.StreamStateChecker.Options.GetOnlineTime,
                                                                                CliNodePermission.ReadOnly,
                                                                                isUnixEpoch: false
                                                                               ),
                                                                           new CliNodeTime(
                                                                                "Offline Time",
                                                                                _state.Data.StreamStateChecker.Options.GetOfflineTime,
                                                                                CliNodePermission.ReadOnly,
                                                                                isUnixEpoch: false
                                                                               ),
                                                                        ]
                                                                        ),
                                                               ]
                                                               );
        
        var tgNotificationsDir = new CliNodeStaticDirectory(
                                                            ServiceName.TgNotifications,
                                                            _state,
                                                            true,
                                                            [
                                                                new CliNodeLong(
                                                                                "Chat Id",
                                                                                _state.Data.TgNotifications.GetChatId,
                                                                                CliNodePermission.Default,
                                                                                _state.Data.TgNotifications.SetChatId
                                                                               ),
                                                                new CliNodeString(
                                                                                "Notification Prompt",
                                                                                _state.Data.TgNotifications.GetNotificationPrompt,
                                                                                CliNodePermission.Default,
                                                                                _state.Data.TgNotifications.SetNotificationPrompt
                                                                               ),
                                                                new CliNodeTime(
                                                                                "Cooldown", 
                                                                                _state.Data.TgNotifications.GetCooldown, 
                                                                                CliNodePermission.Default, 
                                                                                _state.Data.TgNotifications.Options.SetCooldown,
                                                                                isUnixEpoch: false
                                                                                ),
                                                                new CliNodeStaticDirectory(
                                                                                           "Secret",
                                                                                           _state,
                                                                                           true,
                                                                                           [
                                                                                               new CliNodeString(
                                                                                                    "Bot Token",
                                                                                                    _state.Data.TgNotifications.GetBotToken,
                                                                                                    CliNodePermission.Default,
                                                                                                    _state.Data.TgNotifications.SetBotToken
                                                                                                   ),
                                                                                           ]
                                                                                           ),
                                                                new CliNodeEnum(
                                                                                "Service State",
                                                                                _state.Data.TgNotifications.GetServiceStateAsInt,
                                                                                typeof(State),
                                                                                CliNodePermission.Default,
                                                                                _state.Data.TgNotifications.ServiceStateNext
                                                                                ),
                                                            ]
                                                            );
        
        var shopDir = new CliNodeStaticDirectory(
                                                 "Shop", 
                                                 _state, 
                                                 true, 
                                                 []
                                                 );

        var lots = _state.Data.Shop.Lots;
        foreach (var lot in lots) {
            shopDir.AddNode(
                                  new CliNodeStaticDirectory(
                                                             lot.Name,
                                                             _state,
                                                             true,
                                                             [
                                                                 new CliNodeString(
                                                                      "Name",
                                                                      lot.GetName,
                                                                      CliNodePermission.Default,
                                                                      lot.ChangeName
                                                                     ),
                                                                 new CliNodeLong(
                                                                      "Cost",
                                                                      lot.GetCost,
                                                                      CliNodePermission.Default,
                                                                      lot.ChangeCost
                                                                     ),
                                                                 new CliNodeEnum(
                                                                      "State",
                                                                      lot.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      lot.StateNext
                                                                     ),
                                                             ],
                                                             lot.GetName
                                                            )
                                  );
        }

        var bankDir = new CliNodeStaticDirectory(
                                                 ServiceName.Bank,
                                                 _state,
                                                 true,
                                                 [
                                                 new CliNodeLong(
                                                                 "Money Supply",
                                                                 _state.Data.Bank.GetMoneySupply,
                                                                 CliNodePermission.ReadOnly
                                                                 ),
                                                 ]
                                                 );
        
        var casinoDir = new CliNodeStaticDirectory(
                                                 ServiceName.Casino,
                                                 _state,
                                                 true,
                                                 [
                                                     new CliNodeEnum(
                                                                     "State",
                                                                     _state.Data.Casino.GetServiceStateAsInt,
                                                                     typeof(State),
                                                                     CliNodePermission.Default,
                                                                     _state.Data.Casino.ServiceStateNext
                                                                    ),
                                                 ]
                                                );
        
        var pointsDir = new CliNodeStaticDirectory(
                                                   "Points",
                                                   _state,
                                                   true,
                                                   [
                                                       bankDir,
                                                       shopDir,
                                                       casinoDir,
                                                       new CliNodeEnum(
                                                                       "Service State",
                                                                       _state.Data.Casino.GetServiceStateAsInt,
                                                                       typeof(State),
                                                                       CliNodePermission.Default,
                                                                       _state.Data.Casino.ServiceStateNext
                                                                      ),
                                                   ]
                                                   );
        
        var presetsDir = new CliNodeStaticDirectory(
                                                    ServiceName.Presets,
                                                    _state,
                                                    true,
                                                    [
                                                    new CliNodeIndex(
                                                                     "Current Preset",
                                                                     _state.Data.Presets.Options.GetCurrentPreset,
                                                                     CliNodePermission.Default,
                                                                     _state.Data.Presets.SetCurrentPreset
                                                                     ),
                                                    new CliNodeDynamicPresetsDirectory(
                                                                         "Presets",
                                                                         "Add Preset",
                                                                         "Remove Preset",
                                                                         _state
                                                                        ),
                                                    ]
                                                    );

        var debugDir = new CliNodeStaticDirectory(
                                                  "Debug",
                                                  _state,
                                                  true,
                                                  [
                                                      streamStateCheckerDir,
                                                      loggerDir,
                                                  ]
                                                  );

        var coreServicesDir = new CliNodeStaticDirectory(
                                                         "Core",
                                                         _state,
                                                         true,
                                                         [
                                                             chatCmdsDir,
                                                             chatLogsDir,
                                                             messageFilterDir,
                                                             moderationDir,
                                                         ]
                                                         );
        
        var usefulServicesDir = new CliNodeStaticDirectory(
                                                           "Useful",
                                                           _state,
                                                           true,
                                                           [
                                                               aiDir,
                                                               translatorDir,
                                                               demonListDir,
                                                               levelReqsDir,
                                                               gameReqsDir,
                                                               tgNotificationsDir,
                                                           ]
                                                          );
        
        var funServicesDir = new CliNodeStaticDirectory(
                                                        "Fun",
                                                        _state,
                                                        true,
                                                        [
                                                            randomMsgsDir,
                                                            textGeneratorDir,
                                                            pointsDir,
                                                        ]
                                                       );
        
        var servicesDir = new CliNodeStaticDirectory(
                                                  "Services",
                                                  _state,
                                                  true,
                                                  [
                                                      coreServicesDir,
                                                      usefulServicesDir,
                                                      funServicesDir,
                                                      debugDir,
                                                  ]);
        
        var loginDir = new CliNodeStaticDirectory(
                                                  "Credentials",
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeString(
                                                                        "Channel",
                                                                        _state.Data.Bot.GetChannel,
                                                                        CliNodePermission.Default,
                                                                        _state.Data.Bot.SetChannel
                                                                       ),
                                                      new CliNodeStaticDirectory(
                                                                                 "Secret",
                                                                                 _state,
                                                                                 true,
                                                                                 [
                                                                                     new CliNodeString(
                                                                                          "OAuth",
                                                                                          _state.Data.Bot.GetOauth,
                                                                                          CliNodePermission.Default,
                                                                                          _state.Data.Bot.SetOauth
                                                                                         ),
                                                                                     new CliNodeString(
                                                                                          "Broadcaster OAuth",
                                                                                          _state.Data.Bot.GetChannelOauth,
                                                                                          CliNodePermission.Default,
                                                                                          _state.Data.Bot.SetChannelOauth
                                                                                         ),
                                                                                 ]
                                                                                 ),
                                                      new CliNodeAction(
                                                                        "Save",
                                                                        _state.Data.Bot.Options.Save
                                                                       ),
                                                  ]
                                                 );

        var botDir = new CliNodeStaticDirectory(
                                                "Bot",
                                                _state,
                                                true,
                                                [
                                                    loginDir,
                                                    new CliNodeActionAsync(
                                                                      "Initialize",
                                                                      _state.Data.Bot.StartAsync
                                                                     ),
                                                    new CliNodeAction(
                                                                      "Stop",
                                                                      _state.Data.Bot.Stop
                                                                     ),
                                                ]
                                                );
        
        var rootDir = new CliNodeStaticDirectory(
                                                 "Root",
                                                 _state,
                                                 false,
                                                 [
                                                     presetsDir,
                                                     botDir,
                                                     servicesDir,
                                                 ]);

        _directories.Add(rootDir);
    }
}