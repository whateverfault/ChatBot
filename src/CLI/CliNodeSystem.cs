using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.emotes.data;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.localization.data;
using ChatBot.bot.services.scopes;
using ChatBot.bot.services.translator;
using ChatBot.bot.shared.handlers;
using ChatBot.cli.data.CliNodes;
using ChatBot.cli.data.CliNodes.Directories;
using ChatBot.cli.data.CliNodes.Directories.ChatAds;
using ChatBot.cli.data.CliNodes.Directories.ChatCommands;
using ChatBot.cli.data.CliNodes.Directories.MessageFilter;
using ChatBot.cli.data.CliNodes.Directories.Moderation;
using ChatBot.cli.data.CliNodes.Directories.Presets;
using ChatBot.cli.data.CliNodes.Directories.Users;
using TwitchAPI.client;

namespace ChatBot.cli;

public class CliNodeSystem {
    private readonly CliState _state;
    private readonly List<CliNodeDirectory> _directories;
    private int _dirPointer;
    
    public CliNodeDirectory Current => _directories[_dirPointer];
    
    
    public CliNodeSystem(CliState state) {
        _directories = [];
        _state = state;
    }
    
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
        if (_state.Renderer == null) {
            return;
        }
        
        var chatLogsDir = new CliNodeStaticDirectory(
                                                     "Chat Logs",
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
                                                       "Message Randomizer",
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
                                                          "Text Generator",
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
                                                    "Chat Ads",
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

        var emotesDir = new CliNodeStaticDirectory(
                                                   "Emotes",
                                                   _state,
                                                   true,
                                                   []
                                                   );
        
        foreach (var (_, emote) in _state.Data.Emotes.Options.Emotes) {
            if (string.IsNullOrEmpty(emote.Text)) continue;
            emotesDir.AddNode(
                              new CliNodeStaticDirectory(
                                                         emote.Id.ToString(),
                                                         _state,
                                                         true,
                                                         [
                                                             new CliNodeEnum(
                                                                               "Type",
                                                                               emote.GetIdAsInt,
                                                                               typeof(EmoteId),
                                                                               CliNodePermission.ReadOnly
                                                                              ),
                                                             new CliNodeString(
                                                                               "Text",
                                                                               emote.GetText,
                                                                               CliNodePermission.Default,
                                                                               emote.SetText
                                                                              ),
                                                         ])
                             );
        }
        
        var defaultCmdsDir = new CliNodeStaticDirectory(
                                                     "Default Commands",
                                                     _state,
                                                     true,
                                                     []
                                                     );
        
        foreach (var cmd in _state.Data.ChatCommands.Options.DefaultCmds) {
            if (string.IsNullOrEmpty(cmd.Name)) continue;
            defaultCmdsDir.AddNode(
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
                                                                    CliNodePermission.ReadOnly
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
                                                     "Chat Commands",
                                                      _state, 
                                                      true,
                                                      [
                                                          new CliNodeChar(
                                                                          "Command Identifier",
                                                                          _state.Data.ChatCommands.Options.GetCommandIdentifier,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.ChatCommands.SetCommandIdentifier
                                                                          ), 
                                                          defaultCmdsDir,
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
                                                                                        new CliNodeBool(
                                                                                             "Send Whisper If Possible",
                                                                                             _state.Data.ChatCommands.GetSendWhisperIfPossible,
                                                                                             CliNodePermission.Default,
                                                                                             _state.Data.ChatCommands.SetSendWhisperIfPossibleState
                                                                                            ),
                                                                                        new CliNodeBool(
                                                                                             "Use 7tv",
                                                                                             _state.Data.Emotes.Options.GetUse7Tv,
                                                                                             CliNodePermission.Default,
                                                                                             _state.Data.Emotes.Options.SetUse7Tv
                                                                                            ),
                                                                                        emotesDir,
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
                                                       "Moderation",
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
        
        var globalFiltersDir = new CliNodeMessageFilterDynamicDirectory(
                                                                      "Global Filters",
                                                                      "Add Filter",
                                                                      "Remove Filter",
                                                                      _state
                                                                     );

        var bannedUsersDir = new CliNodeUsersDynamicDirectory(
                                                              "Banned Users",
                                                              _state,
                                                              _state.Data.MessageFilter.Options.Save,
                                                              value => {
                                                                  _ = _state.Data.MessageFilter.AddBannedUser(value);
                                                              },
                                                              _state.Data.MessageFilter.RemoveBannedUser,
                                                              _state.Data.MessageFilter.SubscribeToBannedUserAdded,
                                                              _state.Data.MessageFilter.SubscribeToBannedUserRemoved,
                                                              _state.Data.MessageFilter.GetBannedUsers()
                                                              );
        
        var messageFilterDir = new CliNodeStaticDirectory(
                                                  "Message Filter",
                                                  _state,
                                                  true,
                                                  [
                                                      globalFiltersDir,
                                                      bannedUsersDir,
                                                  ]);

        var loggerDir = new CliNodeStaticDirectory(
                                                   "Logger",
                                                   _state,
                                                   true,
                                                   [
                                                       new CliNodeLogDirectory(
                                                                               "Logs",
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
                                                      "Demon List",
                                                      _state,
                                                      true,
                                                      [
                                                          new CliNodeString(
                                                                          "Default Profile",
                                                                          _state.Data.DemonList.Options.GetDefaultUserName,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.DemonList.Options.SetDefaultUserName
                                                                         ),
                                                          new CliNodeBool(
                                                                          "Use Default Profile",
                                                                          _state.Data.DemonList.Options.GetUseDefaultUserName,
                                                                          CliNodePermission.Default,
                                                                          _state.Data.DemonList.Options.SetUseDefaultUserName
                                                                         ), 
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
                                               "Ai",
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
                                                       "Translator",
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
                                                     "Game Requests",
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
                                                               "Stream State Checker",
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
                                                            "Telegram Notifications",
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

        var botLifecycleDir = new CliNodeStaticDirectory(
                                                         "Lifecycle",
                                                         _state,
                                                         true,
                                                         [
                                                             new CliNodeTime(
                                                                             "Disconnect Timeout",
                                                                             _state.Data.BotLifecycle.Options.GetDisconnectTimeout,
                                                                             CliNodePermission.Default,
                                                                             _state.Data.BotLifecycle.Options.SetDisconnectTimeout
                                                                             ),
                                                             new CliNodeBool(
                                                                             "Active",
                                                                             _state.Data.BotLifecycle.BotOnline,
                                                                             CliNodePermission.ReadOnly
                                                                            ),
                                                             new CliNodeEnum(
                                                                             "Service State",
                                                                             _state.Data.BotLifecycle.GetServiceStateAsInt,
                                                                             typeof(State),
                                                                             CliNodePermission.Default,
                                                                             _state.Data.BotLifecycle.ServiceStateNext
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
                                                 "Bank",
                                                 _state,
                                                 true,
                                                 [
                                                 new CliNodeLong(
                                                                 "Money Supply",
                                                                 _state.Data.Bank.GetMoneySupply,
                                                                 CliNodePermission.ReadOnly
                                                                 ),
                                                 new CliNodeStaticDirectory(
                                                                            "Account Filtering",
                                                                            _state,
                                                                            true,
                                                                            [
                                                                                new CliNodeLong(
                                                                                     "Minimum Stream Length",
                                                                                     _state.Data.BankAccountFiltering.Options.GetMinStreamLength,
                                                                                     CliNodePermission.Default,
                                                                                     _state.Data.BankAccountFiltering.Options.SetMinStreamLength
                                                                                     ),
                                                                                new CliNodeLong(
                                                                                     "Last Active Threshold",
                                                                                     _state.Data.BankAccountFiltering.Options.GetLastActiveThreshold,
                                                                                     CliNodePermission.Default,
                                                                                     _state.Data.BankAccountFiltering.Options.SetLastActiveThreshold
                                                                                    ),
                                                                                new CliNodeLong(
                                                                                     "Passed Streams Threshold",
                                                                                     _state.Data.BankAccountFiltering.Options.GetPassedStreamsThreshold,
                                                                                     CliNodePermission.Default,
                                                                                     _state.Data.BankAccountFiltering.Options.SetPassedStreamsThreshold
                                                                                    ),
                                                                                new CliNodeEnum(
                                                                                     "Service State",
                                                                                     _state.Data.BankAccountFiltering.GetServiceStateAsInt,
                                                                                     typeof(State),
                                                                                     CliNodePermission.Default,
                                                                                     _state.Data.BankAccountFiltering.ServiceStateNext
                                                                                    ),
                                                                            ]
                                                                            ),
                                                 ]
                                                 );
        
        var casinoDir = new CliNodeStaticDirectory(
                                                 "Casino",
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
                                                    new CliNodeDynamicPresetsDirectory(
                                                                         "Presets",
                                                                         "Add Preset",
                                                                         "Remove Preset",
                                                                         _state
                                                                        ),
                                                    ]
                                                    );

        var rendererDir = new CliNodeStaticDirectory(
                                                     "Renderer",
                                                     _state,
                                                     true,
                                                     [
                                                         new CliNodeAction(
                                                                           "Change Renderer",
                                                                           _state.Renderer.RendererNext
                                                                           ),
                                                         new CliNodeEnum(
                                                                           "Current Renderer",
                                                                           _state.Renderer.GetRendererAsInt,
                                                                           typeof(Renderer),
                                                                           CliNodePermission.ReadOnly
                                                                          ),
                                                     ]
                                                     );
        
        var debugDir = new CliNodeStaticDirectory(
                                                  "Debug",
                                                  _state,
                                                  true,
                                                  [
                                                      loggerDir,
                                                      streamStateCheckerDir,
                                                      rendererDir,
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
                                                               botLifecycleDir,
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
                                                                        value => {
                                                                            _ = _state.Data.Bot.SetChannel(value);
                                                                        }),
                                                      new CliNodeStaticDirectory(
                                                           "Authorization", 
                                                           _state,
                                                           true,
                                                           [
                                                               new CliNodeAction(
                                                                    "Authorize", 
                                                                    () => _ = _state.Data.Bot.GetAuthorization() 
                                                                    ),
                                                               new CliNodeEnum(
                                                                               "Authorize As",
                                                                               _state.Data.Scopes.GetScopesPresetAsInt,
                                                                               typeof(ScopesPreset),
                                                                               CliNodePermission.Default,
                                                                               _state.Data.Scopes.ScopesPresetNext
                                                                              ),
                                                               new CliNodeStaticDirectory(
                                                                    "Other",
                                                                    _state,
                                                                    true, 
                                                                    [
                                                                        new CliNodeEnum(
                                                                             "Authorization Level", 
                                                                             _state.Data.Bot.GetAuthLevelAsInt, 
                                                                             typeof(AuthLevel), 
                                                                             CliNodePermission.ReadOnly
                                                                            ),
                                                                        new CliNodeString(
                                                                             "Authorized Broadcaster", 
                                                                             _state.Data.Bot.GetAuthorizedBroadcasterDisplayName, 
                                                                             CliNodePermission.ReadOnly
                                                                             ),
                                                                    ]
                                                               ),
                                                           ]
                                                      ),
                                                  ]
                                                 );

        var botDir = new CliNodeStaticDirectory(
                                                "Bot",
                                                _state,
                                                true,
                                                [
                                                    new CliNodeEnum(
                                                                    "Language",
                                                                    _state.Data.Localization.GetLanguageAsInt,
                                                                    typeof(Lang),
                                                                    CliNodePermission.Default,
                                                                    _state.Data.Localization.LanguageNext),
                                                    loginDir,
                                                    new CliNodeAction(
                                                                      "Initialize",
                                                                      () => _ = _state.Data.Bot.Start()
                                                                     ),
                                                    new CliNodeAction(
                                                                      "Stop",
                                                                      () => {
                                                                          _ = _state.Data.Bot.Stop();
                                                                      }),
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