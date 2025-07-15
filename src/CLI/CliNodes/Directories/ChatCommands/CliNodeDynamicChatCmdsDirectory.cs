using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_commands.Data;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.CLI.CliNodes.Directories.ChatCommands;

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
        AddChatCmdHandler addHandler,
        RemoveHandler removeHandler,
        List<CustomChatCommand> commands,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = addHandler;
        _removeHandler = removeHandler;

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

        foreach (var cmd in commands) {
            _content.AddNode(CommandToNode(cmd));
        }
        
        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeChatCmdAdd(addText, Add),
                    new CliNodeRemove(removeText, Remove),
                    _content,
                ];
    }
    
    private void Add(ChatCommand chatCmd) {
        if (chatCmd.GetType() != typeof(CustomChatCommand)) return;

        var cmd = (CustomChatCommand)chatCmd;
        var node = CommandToNode(cmd);
        
        _content.AddNode(node);
        _addHandler.Invoke(chatCmd);
    }
    
    private void Remove(int index) {
        if (index < 0 || index >= _content.Nodes.Count-2) {
            return;
        }
        
        _content.RemoveNode(index+2);
        _removeHandler.Invoke(index);
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