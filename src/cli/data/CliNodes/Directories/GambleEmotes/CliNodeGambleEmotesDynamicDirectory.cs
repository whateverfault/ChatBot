using ChatBot.bot.services.casino.data;

namespace ChatBot.cli.data.CliNodes.Directories.GambleEmotes;

public class CliNodeGambleEmotesDynamicDirectory : CliNodeDirectory {
    private readonly AddGambleEmoteHandler _addHandler;
    private readonly RemoveGambleEmoteHandler _removeHandler;

    private readonly CliState _state;

    public override string Text { get; }

    public override List<CliNode> Nodes { get; }
    public override bool HasBackOption => true;


    public CliNodeGambleEmotesDynamicDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        _addHandler = state.Data.Casino.Options.AddEmote;
        _removeHandler = state.Data.Casino.Options.RemoveEmote;
        _state = state;
        Text = text;

        
        var content = new CliNodeStaticDirectory(
                                                 "Content",
                                                 _state,
                                                 true,
                                                 [
                                                     new CliNodeText(
                                                                     "-----------------------------------",
                                                                     false,
                                                                     true,
                                                                     1
                                                                    ),
                                                 ]
                                                );
        
        var emotes = _state.Data.Casino.Options.Emotes;
        foreach (var emote in emotes) {
            content.AddNode(EmoteToNode(emote));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeGambleEmoteAdd(addText, Add),
                        new CliNodeGambleEmoteRemove(removeText, Remove),
                        content,
                    ];
        }

        _state.Data.Casino.Options.OnEmoteAdded += (_, emote) => { 
                                                       content.AddNode(EmoteToNode(emote));
                                                   };
        
        _state.Data.Casino.Options.OnEmoteRemoved += (_, name) => {
                                                         var index = content.Nodes.FindIndex(x => x.Text == name);
                                                         if (index < 0) return;
                                                         
                                                          content.RemoveNode(index);
                                                      };
    }

    private bool Add(string name) {
        return _addHandler.Invoke(name);
    }

    private bool Remove(string name) {
        return _removeHandler.Invoke(name);
    }

    private CliNodeStaticDirectory EmoteToNode(GambleEmote emote) {
        var node = new CliNodeStaticDirectory(
                                              emote.Name,
                                              _state,
                                              true,
                                              [
                                                  new CliNodeString(
                                                                    "Name",
                                                                    emote.GetText,
                                                                    CliNodePermission.Default,
                                                                    emote.SetText
                                                                   ),
                                                  new CliNodeDouble(
                                                                    "Part",
                                                                    emote.GetPart,
                                                                    CliNodePermission.Default,
                                                                    emote.SetPart
                                                                   ),
                                                  new CliNodeDouble(
                                                                    "Combo Coefficient",
                                                                    emote.GetComboCoefficient,
                                                                    CliNodePermission.Default,
                                                                    emote.SetComboCoefficient
                                                                   ),
                                                  new CliNodeDouble(
                                                                    "Bonus Coefficient",
                                                                    emote.GetBonusCoefficient,
                                                                    CliNodePermission.Default,
                                                                    emote.SetBonusCoefficient
                                                                   ),
                                                  new CliNodeInt(
                                                                 "Combo",
                                                                 emote.GetAmountForCombo,
                                                                 CliNodePermission.Default,
                                                                 emote.SetAmountForCombo
                                                                 ),
                                              ],
                                              emote.GetText
                                             );
        return node;
    }
}