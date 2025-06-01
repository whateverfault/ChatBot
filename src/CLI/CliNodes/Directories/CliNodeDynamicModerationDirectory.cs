using ChatBot.Services.message_filter;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.CLI.CliNodes.Directories;

public class CliNodeDynamicModerationDirectory : CliNodeDirectory {
    private readonly MessageFilterService _messageFilter;
    private readonly AddModActionHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    private readonly CliNodeStaticDirectory _dynamicDir;
    private readonly CliState _state;

    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }


    public CliNodeDynamicModerationDirectory(
        string text,
        string addText,
        string removeText,
        ModerationActionType actionType,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = _state.Data.Moderation.AddModAction;
        _removeHandler = _state.Data.Moderation.RemoveModAction;

        _dynamicDir = new CliNodeStaticDirectory(
                                                 "Content",
                                                 state,
                                                 true,
                                                 []
                                                );

        _dynamicDir.AddNode(
                            new CliNodeText(
                                            "-----------------------------------",
                                            false,
                                            true,
                                            1
                                            )
                            );

        _messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);

        var nodesContent = _state.Data.Moderation.GetModActions();
        
        foreach (var nodeContent in nodesContent) {
            if (nodeContent.Type != actionType) continue;

            switch (nodeContent.Type) {
                case ModerationActionType.Ban:
                    _dynamicDir.AddNode(
                                        new CliNodeStaticDirectory
                                            (
                                             _messageFilter.GetPatternWithComment(nodeContent.PatternIndex).Comment,
                                             _state,
                                             true,
                                             [
                                                 new CliNodeInt(
                                                                "Global Pattern Index",
                                                                nodeContent.GetIndex,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetIndex
                                                               ),
                                                 new CliNodeEnum(
                                                                 "Exclude Roles",
                                                                 nodeContent.GetRestrictionAsInt,
                                                                 typeof(Restriction),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.RestrictionNext
                                                                ),
                                                 new CliNodeString(
                                                                   "Moderator Comment",
                                                                   nodeContent.GetComment,
                                                                   CliNodePermission.Default,
                                                                   nodeContent.SetComment
                                                                  ),
                                                 new CliNodeEnum(
                                                                 "State",
                                                                 nodeContent.GetStateAsInt,
                                                                 typeof(State),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.StateNext
                                                                ),
                                             ]
                                            )
                                       );
                    break;
                case ModerationActionType.Timeout:
                    _dynamicDir.AddNode(
                                        new CliNodeStaticDirectory
                                            (
                                             _messageFilter.GetPatternWithComment(nodeContent.PatternIndex).Comment,
                                             _state,
                                             true,
                                             [
                                                 new CliNodeInt(
                                                                "Global Pattern Index",
                                                                nodeContent.GetIndex,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetIndex
                                                               ),
                                                 new CliNodeInt(
                                                                "Duration",
                                                                nodeContent.GetDuration,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetDuration
                                                               ),
                                                 new CliNodeEnum(
                                                                 "Exclude Roles",
                                                                 nodeContent.GetRestrictionAsInt,
                                                                 typeof(Restriction),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.RestrictionNext
                                                                ),
                                                 new CliNodeString(
                                                                   "Moderator Comment",
                                                                   nodeContent.GetComment,
                                                                   CliNodePermission.Default,
                                                                   nodeContent.SetComment
                                                                  ),
                                                 new CliNodeEnum(
                                                                 "State",
                                                                 nodeContent.GetStateAsInt,
                                                                 typeof(State),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.StateNext
                                                                ),
                                             ]
                                            )
                                       );
                    break;
                case ModerationActionType.Warn:
                    _dynamicDir.AddNode(
                                        new CliNodeStaticDirectory
                                            (
                                             _messageFilter.GetPatternWithComment(nodeContent.PatternIndex).Comment,
                                             _state,
                                             true,
                                             [
                                                 new CliNodeInt(
                                                                "Global Pattern Index",
                                                                nodeContent.GetIndex,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetIndex
                                                               ),
                                                 new CliNodeEnum(
                                                                 "Exclude Roles",
                                                                 nodeContent.GetRestrictionAsInt,
                                                                 typeof(Restriction),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.RestrictionNext
                                                                ),
                                                 new CliNodeString(
                                                                   "Moderator Comment",
                                                                   nodeContent.GetComment,
                                                                   CliNodePermission.Default,
                                                                   nodeContent.SetComment
                                                                  ),
                                                 new CliNodeEnum(
                                                                 "State",
                                                                 nodeContent.GetStateAsInt,
                                                                 typeof(State),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.StateNext
                                                                ),
                                             ]
                                            )
                                       );
                    break;
                case ModerationActionType.WarnWithTimeout:
                    _dynamicDir.AddNode(
                                        new CliNodeStaticDirectory
                                            (
                                             _messageFilter.GetPatternWithComment(nodeContent.PatternIndex).Comment,
                                             _state,
                                             true,
                                             [
                                                 new CliNodeInt(
                                                                "Global Pattern Index",
                                                                nodeContent.GetIndex,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetIndex
                                                               ),
                                                 new CliNodeInt(
                                                                "Max Warns",
                                                                nodeContent.GetMaxWarnCount,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetMaxWarnCount
                                                               ),
                                                 new CliNodeInt(
                                                                "Duration",
                                                                nodeContent.GetDuration,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetDuration
                                                               ),
                                                 new CliNodeEnum(
                                                                 "Exclude Roles",
                                                                 nodeContent.GetRestrictionAsInt,
                                                                 typeof(Restriction),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.RestrictionNext
                                                                ),
                                                 new CliNodeString(
                                                                   "Moderator Comment",
                                                                   nodeContent.GetComment,
                                                                   CliNodePermission.Default,
                                                                   nodeContent.SetComment
                                                                  ),
                                                 new CliNodeEnum(
                                                                 "State",
                                                                 nodeContent.GetStateAsInt,
                                                                 typeof(State),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.StateNext
                                                                ),
                                             ]
                                            )
                                       );
                    break;
                case ModerationActionType.WarnWithBan:
                    _dynamicDir.AddNode(
                                        new CliNodeStaticDirectory
                                            (
                                             _messageFilter.GetPatternWithComment(nodeContent.PatternIndex).Comment,
                                             _state,
                                             true,
                                             [
                                                 new CliNodeInt(
                                                                "Global Pattern Index",
                                                                nodeContent.GetIndex,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetIndex
                                                               ),
                                                 new CliNodeInt(
                                                                "Max Warns",
                                                                nodeContent.GetMaxWarnCount,
                                                                CliNodePermission.Default,
                                                                nodeContent.SetMaxWarnCount
                                                               ),
                                                 new CliNodeEnum(
                                                                 "Exclude Roles",
                                                                 nodeContent.GetRestrictionAsInt,
                                                                 typeof(Restriction),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.RestrictionNext
                                                                ),
                                                 new CliNodeString(
                                                                   "Moderator Comment",
                                                                   nodeContent.GetComment,
                                                                   CliNodePermission.Default,
                                                                   nodeContent.SetComment
                                                                  ),
                                                 new CliNodeEnum(
                                                                 "State",
                                                                 nodeContent.GetStateAsInt,
                                                                 typeof(State),
                                                                 CliNodePermission.Default,
                                                                 nodeContent.StateNext
                                                                ),
                                             ]
                                            )
                                       );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeModActionAdd(addText, Add, actionType),
                    new CliNodeRemove(removeText, Remove),
                    _dynamicDir,
                ];
    }
    
    private void Add(ModAction modAction) {
        CliNodeStaticDirectory node;
        
        switch (modAction.Type) {
            case ModerationActionType.Ban: {
                node = new CliNodeStaticDirectory(
                                                  _messageFilter.GetPatternWithComment(modAction.PatternIndex).Comment,
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeInt(
                                                                     "Global Pattern Index",
                                                                     modAction.GetIndex,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetIndex
                                                                    ),
                                                      new CliNodeEnum(
                                                                      "Exclude Roles",
                                                                      modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      modAction.RestrictionNext
                                                                     ),
                                                      new CliNodeString(
                                                                        "Moderator Comment",
                                                                        modAction.GetComment,
                                                                        CliNodePermission.Default,
                                                                        modAction.SetComment
                                                                       ),
                                                      new CliNodeEnum(
                                                                      "State",
                                                                      modAction.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      modAction.StateNext
                                                                     ),
                                                  ],
                                                  _messageFilter.GetComment
                                                 );
                break;
            }
            case ModerationActionType.Timeout: {
                node = new CliNodeStaticDirectory(
                                                  _messageFilter.GetPatternWithComment(modAction.PatternIndex).Comment,
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeInt(
                                                                     "Global Pattern Index",
                                                                     modAction.GetIndex,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetIndex
                                                                    ),
                                                      new CliNodeInt(
                                                                     "Duration",
                                                                     modAction.GetDuration,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetDuration
                                                                    ),
                                                      new CliNodeEnum(
                                                                      "Exclude Roles",
                                                                      modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      modAction.RestrictionNext
                                                                     ),
                                                      new CliNodeString(
                                                                        "Moderator Comment",
                                                                        modAction.GetComment,
                                                                        CliNodePermission.Default,
                                                                        modAction.SetComment
                                                                       ),
                                                      new CliNodeEnum(
                                                                      "State",
                                                                      modAction.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      modAction.StateNext
                                                                     ),
                                                  ],
                                                  _messageFilter.GetComment
                                                 );
                break;
            }
            case ModerationActionType.Warn: {
                                node = new CliNodeStaticDirectory(
                                                  _messageFilter.GetPatternWithComment(modAction.PatternIndex).Comment,
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeInt(
                                                                     "Global Pattern Index",
                                                                     modAction.GetIndex,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetIndex
                                                                    ),
                                                      new CliNodeEnum(
                                                                      "Exclude Roles",
                                                                      modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      modAction.RestrictionNext
                                                                     ),
                                                      new CliNodeString(
                                                                        "Moderator Comment",
                                                                        modAction.GetComment,
                                                                        CliNodePermission.Default,
                                                                        modAction.SetComment
                                                                       ),
                                                      new CliNodeEnum(
                                                                      "State",
                                                                      modAction.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      modAction.StateNext
                                                                     ),
                                                  ],
                                                  _messageFilter.GetComment
                                                 );
                break;
            }
            case ModerationActionType.WarnWithTimeout:
                                node = new CliNodeStaticDirectory(
                                                  _messageFilter.GetPatternWithComment(modAction.PatternIndex).Comment,
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeInt(
                                                                     "Global Pattern Index",
                                                                     modAction.GetIndex,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetIndex
                                                                    ),
                                                      new CliNodeInt(
                                                                     "Max Warns",
                                                                     modAction.GetMaxWarnCount,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetMaxWarnCount
                                                                    ),
                                                      new CliNodeInt(
                                                                     "Duration",
                                                                     modAction.GetDuration,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetDuration
                                                                    ),
                                                      new CliNodeEnum(
                                                                      "Exclude Roles",
                                                                      modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      modAction.RestrictionNext
                                                                     ),
                                                      new CliNodeString(
                                                                        "Moderator Comment",
                                                                        modAction.GetComment,
                                                                        CliNodePermission.Default,
                                                                        modAction.SetComment
                                                                       ),
                                                      new CliNodeEnum(
                                                                      "State",
                                                                      modAction.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      modAction.StateNext
                                                                     ),
                                                  ],
                                                  _messageFilter.GetComment
                                                 );
                break;
            case ModerationActionType.WarnWithBan:
                                node = new CliNodeStaticDirectory(
                                                  _messageFilter.GetPatternWithComment(modAction.PatternIndex).Comment,
                                                  _state,
                                                  true,
                                                  [
                                                      new CliNodeInt(
                                                                     "Global Pattern Index",
                                                                     modAction.GetIndex,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetIndex
                                                                    ),
                                                      new CliNodeInt(
                                                                     "Max Warns",
                                                                     modAction.GetMaxWarnCount,
                                                                     CliNodePermission.Default,
                                                                     modAction.SetMaxWarnCount
                                                                    ),
                                                      new CliNodeEnum(
                                                                      "Exclude Roles",
                                                                      modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction),
                                                                      CliNodePermission.Default,
                                                                      modAction.RestrictionNext
                                                                     ),
                                                      new CliNodeString(
                                                                        "Moderator Comment",
                                                                        modAction.GetComment,
                                                                        CliNodePermission.Default,
                                                                        modAction.SetComment
                                                                       ),
                                                      new CliNodeEnum(
                                                                      "State",
                                                                      modAction.GetStateAsInt,
                                                                      typeof(State),
                                                                      CliNodePermission.Default,
                                                                      modAction.StateNext
                                                                     ),
                                                  ],
                                                  _messageFilter.GetComment
                                                 );
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _dynamicDir.AddNode(node);
        _addHandler.Invoke(modAction);
    }
    
    private void Remove(int index) {
        if (index < 0 || index >= _dynamicDir.Nodes.Count-2) {
            return;
        }
        
        _dynamicDir.RemoveNode(index+2);
        _removeHandler.Invoke(index);
    }
}