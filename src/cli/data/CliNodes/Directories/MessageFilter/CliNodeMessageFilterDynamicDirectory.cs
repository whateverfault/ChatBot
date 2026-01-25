using ChatBot.bot.services.message_filter.data;

namespace ChatBot.cli.data.CliNodes.Directories.MessageFilter;

public class CliNodeMessageFilterDynamicDirectory : CliNodeDirectory {
    private readonly AddFilterHandler _addHandler;
    private readonly RemoveHandler _removeHandler;

    private readonly CliState _state;

    protected override string Text { get; }

    public override List<CliNode> Nodes { get; }
    public override bool HasBackOption => true;


    public CliNodeMessageFilterDynamicDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        _addHandler = state.Data.MessageFilter.AddFilter;
        _removeHandler = state.Data.MessageFilter.RemoveFilter;
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
        
        var filters = _state.Data.MessageFilter.GetFilters();
        foreach (var filter in filters) {
            content.AddNode(FilterToNode(filter));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeFilterAdd(addText, Add),
                        new CliNodeRemove(removeText, Remove),
                        content,
                    ];
        }

        _state.Data.MessageFilter.OnFilterAdded += (_, filter) => { 
                                                       content.AddNode(FilterToNode(filter));
                                                   };
        
        _state.Data.MessageFilter.OnFilterRemoved += (_, index) => { 
                                                         content.RemoveNode(index+2);
                                                   };
    }

    private void Add(Filter filter) {
        _addHandler.Invoke(filter);
    }

    private bool Remove(int index) {
        return _removeHandler.Invoke(index);
    }

    private CliNodeStaticDirectory FilterToNode(Filter filter) {
        var node = new CliNodeStaticDirectory(
                                              filter.Name,
                                              _state,
                                              true,
                                              [
                                                  new CliNodeString(
                                                                    "Name",
                                                                    filter.GetName,
                                                                    CliNodePermission.Default,
                                                                    filter.SetName
                                                                   ),
                                                  new CliNodeString(
                                                                    "Pattern",
                                                                    filter.GetPattern,
                                                                    CliNodePermission.Default,
                                                                    filter.SetPattern
                                                                   ),
                                              ],
                                              filter.GetName
                                             );
        return node;
    }
}