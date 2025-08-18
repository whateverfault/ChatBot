using ChatBot.bot.services.chat_commands.Data;
using ChatBot.bot.shared.handlers;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.cli.CliNodes.Directories.ChatCommands;

public class CliNodeDynamicChatCmdsDirectory : CliNodeDirectory {
    private readonly AddChatCmdHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    private readonly CliNodeStaticDirectory _content;
    private readonly CliState _state;
    
    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }
    
    
    public CliNodeDynamicChatCmdsDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = state.Data.ChatCommands.AddChatCmd;
        _removeHandler = state.Data.ChatCommands.RemoveChatCmd;

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

        var commands = _state.Data.ChatCommands.GetCustomChatCommands();
        foreach (var cmd in commands) {
            _content.AddNode(CommandToNode(cmd));
        }
        
        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeChatCmdAdd(addText, Add),
                    new CliNodeRemove(removeText, Remove),
                    _content,
                ];

        _state.Data.ChatCommands.OnChatCommandAdded += (_, chatCmd) => {
                                                           _content.AddNode(CommandToNode(chatCmd));
                                                       };

        _state.Data.ChatCommands.OnChatCommandRemoved += (_, index) => {
                                                             _content.RemoveNode(index+2);
                                                         };
    }
    
    private void Add(CustomChatCommand chatCmd) {
        _addHandler.Invoke(chatCmd);
    }
    
    private bool Remove(int index) {
        if (index < 0 || index >= _content.Nodes.Count-2) {
            return false;
        }
        
        return _removeHandler.Invoke(index);
    }
    
    private CliNodeStaticDirectory CommandToNode(CustomChatCommand cmd) {
        var node = new CliNodeStaticDirectory(
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
                                                  new CliNodeBool(
                                                                  "Has Identifier",
                                                                  cmd.GetHasIdentifier,
                                                                  CliNodePermission.Default,
                                                                  cmd.SetHasIdentifier
                                                                  ),
                                                  new CliNodeString(
                                                                    "Output",
                                                                    cmd.GetOutput,
                                                                    CliNodePermission.Default,
                                                                    cmd.SetOutput
                                                                   ),
                                                  new CliNodeInt(
                                                                 "Cooldown",
                                                                 cmd.GetCooldown,
                                                                 CliNodePermission.Default,
                                                                 cmd.SetCooldown
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
                                              ]
                                              );

        return node;
    }
}