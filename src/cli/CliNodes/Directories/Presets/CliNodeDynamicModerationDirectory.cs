using ChatBot.bot.services.moderation;
using ChatBot.bot.services.presets.data;
using ChatBot.bot.services.Static;

namespace ChatBot.cli.CliNodes.Directories.Presets;

public class CliNodeDynamicPresetsDirectory : CliNodeDirectory {
    private readonly ModerationService _moderation;
    
    private readonly AddPreset _addHandler;
    private readonly RemoveHandler _removeHandler;
    
    private readonly CliNodeStaticDirectory _content;
    private readonly CliState _state;

    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }


    public CliNodeDynamicPresetsDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = _state.Data.Presets.AddPreset;
        _removeHandler = _state.Data.Presets.RemovePreset;

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

        _moderation = (ModerationService)Services.Get(ServiceId.Moderation);

        var presets = _state.Data.Presets.GetPresets();
        foreach (var preset in presets) {
            _content.AddNode(PresetToNode(preset));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodePresetAdd(addText, Add),
                        new CliNodeRemove(removeText, Remove),
                        _content,
                    ];
        }
        

        _state.Data.Presets.OnPresetAdded += (_, preset) => {
                                                       _content.AddNode(PresetToNode(preset));
                                                   };
        
        _state.Data.Presets.OnPresetRemoved += (_, index) => {
                                                         _content.RemoveNode(index+2);
                                                   };
    }
    
    private void Add(string name) {
        _addHandler.Invoke(name);
    }
    
    private bool Remove(int index) {
        var modActions = _moderation.GetModActions();

        if (index < 0 || index >= _content.Nodes.Count-2) {
            return false;
        }

        return !modActions[index].IsDefault 
            && _removeHandler.Invoke(index);
    }

    private CliNodeStaticDirectory PresetToNode(Preset preset) {
        var node = new CliNodeStaticDirectory(
                                              preset.Name,
                                              _state,
                                              true,
                                              [
                                                  new CliNodeString(
                                                                    "Name",
                                                                    preset.GetName,
                                                                    CliNodePermission.Default,
                                                                    preset.SetName
                                                                    ),
                                              ],
                                              preset.GetName
                                              );
        return node;
    }
}