﻿using ChatBot.bot.interfaces;
using ChatBot.bot.services.moderation;
using ChatBot.bot.services.moderation.data;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes.Directories.Moderation;

public class CliNodeDynamicModerationDirectory : CliNodeDirectory {
    private readonly ModerationService _moderation;
    
    private readonly AddModActionHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    
    private readonly CliNodeStaticDirectory _content;
    private readonly CliState _state;

    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }


    public CliNodeDynamicModerationDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = _state.Data.Moderation.AddModAction;
        _removeHandler = _state.Data.Moderation.RemoveModAction;

        _content = new CliNodeStaticDirectory(
                                              "Content",
                                              state,
                                              true, 
                                              []
                                              );

        _content.AddNode(
                         new CliNodeText(
                                         "-----------------------------------",
                                         false,
                                         true,
                                         1
                                        )
                         );

        _moderation = (ModerationService)ServiceManager.GetService(ServiceName.Moderation);

        var nodesContent = _state.Data.Moderation.GetModActions();
        foreach (var node in nodesContent) {
            _content.AddNode(ModActionToNode(node));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeModActionAdd(addText, Add),
                        new CliNodeRemove(removeText, Remove),
                        _content,
                    ];
        }

        _state.Data.Moderation.OnModActionAdded += (_, modAction) => {
                                                       _content.AddNode(ModActionToNode(modAction));
                                                   };
        
        _state.Data.Moderation.OnModActionRemoved += (_, index) => {
                                                         _content.RemoveNode(index+2);
                                                   };
    }
    
    private void Add(ModAction modAction) {
        _addHandler.Invoke(modAction);
    }
    
    private bool Remove(int index) {
        var modActions = _moderation.GetModActions();

        if (index < 0 || index >= _content.Nodes.Count-2) {
            return false;
        }

        return !modActions[index].IsDefault 
            && _removeHandler.Invoke(index);
    }

    private CliNodeStaticDirectory ModActionToNode(ModAction modAction) {
        var node = modAction.Type switch {
                       ModerationActionType.Ban => new
                           CliNodeStaticDirectory(modAction.Name, _state,
                                                  true,
                                                  [
                                                      new CliNodeString("Name", modAction.GetName,
                                                                        CliNodePermission.Default, modAction.SetName),
                                                      new CliNodeIndex("Global Filter Index", modAction.GetIndex,
                                                                     CliNodePermission.Default, modAction.SetIndex),
                                                      new CliNodeEnum("Exclude Roles", modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction), CliNodePermission.Default,
                                                                      modAction.RestrictionNext),
                                                      new CliNodeString("Moderator Comment", modAction.GetComment,
                                                                        CliNodePermission.Default, modAction.SetComment),
                                                      new CliNodeEnum("State", modAction.GetStateAsInt, typeof(State),
                                                                      CliNodePermission.Default, modAction.StateNext),
                                                  ], modAction.GetName),
                       ModerationActionType.Timeout => new
                           CliNodeStaticDirectory(modAction.Name, _state,
                                                  true,
                                                  [
                                                      new CliNodeString("Name", modAction.GetName,
                                                                        CliNodePermission.Default, modAction.SetName),
                                                      new CliNodeIndex("Global Filter Index", modAction.GetIndex,
                                                                     CliNodePermission.Default, modAction.SetIndex),
                                                      new CliNodeTime("Duration", modAction.GetDuration,
                                                                     CliNodePermission.Default, modAction.SetDuration,
                                                                      isUnixEpoch: false),
                                                      new CliNodeEnum("Exclude Roles", modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction), CliNodePermission.Default,
                                                                      modAction.RestrictionNext),
                                                      new CliNodeString("Moderator Comment", modAction.GetComment,
                                                                        CliNodePermission.Default, modAction.SetComment),
                                                      new CliNodeEnum("State", modAction.GetStateAsInt, typeof(State),
                                                                      CliNodePermission.Default, modAction.StateNext),
                                                  ], modAction.GetName),
                       ModerationActionType.Warn => new
                           CliNodeStaticDirectory(modAction.Name, _state,
                                                  true,
                                                  [
                                                      new CliNodeString("Name", modAction.GetName,
                                                                        CliNodePermission.Default, modAction.SetName),
                                                      new CliNodeIndex("Global Filter Index", modAction.GetIndex,
                                                                     CliNodePermission.Default, modAction.SetIndex),
                                                      new CliNodeEnum("Exclude Roles", modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction), CliNodePermission.Default,
                                                                      modAction.RestrictionNext),
                                                      new CliNodeString("Moderator Comment", modAction.GetComment,
                                                                        CliNodePermission.Default, modAction.SetComment),
                                                      new CliNodeEnum("State", modAction.GetStateAsInt, typeof(State),
                                                                      CliNodePermission.Default, modAction.StateNext),
                                                  ], modAction.GetName),
                       ModerationActionType.WarnWithTimeout => new
                           CliNodeStaticDirectory(modAction.Name, _state,
                                                  true,
                                                  [
                                                      new CliNodeString("Name", modAction.GetName,
                                                                        CliNodePermission.Default, modAction.SetName),
                                                      new CliNodeIndex("Global Filter Index", modAction.GetIndex,
                                                                       CliNodePermission.Default, modAction.SetIndex),
                                                      new CliNodeInt("Max Warns", modAction.GetMaxWarnCount,
                                                                     CliNodePermission.Default, modAction.SetMaxWarnCount),
                                                      new CliNodeTime("Duration", modAction.GetDuration,
                                                                     CliNodePermission.Default, modAction.SetDuration,
                                                                     isUnixEpoch: false),
                                                      new CliNodeBool("Clear After Stream", modAction.GetClearAfterStream,
                                                                      CliNodePermission.Default, modAction.SetClearAfterStream),
                                                      new CliNodeEnum("Exclude Roles", modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction), CliNodePermission.Default,
                                                                      modAction.RestrictionNext),
                                                      new CliNodeString("Moderator Comment", modAction.GetComment,
                                                                        CliNodePermission.Default, modAction.SetComment),
                                                      new CliNodeEnum("State", modAction.GetStateAsInt, typeof(State),
                                                                      CliNodePermission.Default, modAction.StateNext),
                                                  ], modAction.GetName),
                       ModerationActionType.WarnWithBan => new
                           CliNodeStaticDirectory(modAction.Name, _state,
                                                  true,
                                                  [
                                                      new CliNodeString("Name", modAction.GetName,
                                                                        CliNodePermission.Default, modAction.SetName),
                                                      new CliNodeIndex("Global Filter Index", modAction.GetIndex,
                                                                       CliNodePermission.Default, modAction.SetIndex),
                                                      new CliNodeInt("Max Warns", modAction.GetMaxWarnCount,
                                                                     CliNodePermission.Default, modAction.SetMaxWarnCount),
                                                      new CliNodeBool("Clear After Stream", modAction.GetClearAfterStream,
                                                                      CliNodePermission.Default, modAction.SetClearAfterStream),
                                                      new CliNodeEnum("Exclude Roles", modAction.GetRestrictionAsInt,
                                                                      typeof(Restriction), CliNodePermission.Default,
                                                                      modAction.RestrictionNext),
                                                      new CliNodeString("Moderator Comment", modAction.GetComment,
                                                                        CliNodePermission.Default, modAction.SetComment),
                                                      new CliNodeEnum("State", modAction.GetStateAsInt, typeof(State),
                                                                      CliNodePermission.Default, modAction.StateNext),
                                                  ], modAction.GetName),
                       _ => throw new ArgumentOutOfRangeException(),
                   };
        return node;
    }
}