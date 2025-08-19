using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;

namespace ChatBot.cli.CliNodes.Directories.MessageFilter;

public class CliNodeMessageFilterDynamicDirectory : CliNodeDirectory {
    private readonly MessageFilterService _messageFilter;
    
    private readonly AddFilterHandler _addHandler;
    private readonly RemoveHandler _removeHandler;

    private readonly CliNodeStaticDirectory _content;
    private readonly CliState _state;

    protected override string Text { get; }

    public override List<CliNode> Nodes { get; }


    public CliNodeMessageFilterDynamicDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        _addHandler = state.Data.MessageFilter.AddFilter;
        _removeHandler = state.Data.MessageFilter.RemoveFilter;
        _state = state;
        Text = text;

        
        _content = new CliNodeStaticDirectory(
                                              "Content",
                                              _state,
                                              true,
                                              []
                                              );
        
        _messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);

        var filters = _state.Data.MessageFilter.GetFilters();
        foreach (var filter in filters) {
            _content.AddNode(FilterToNode(filter));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeFilterAdd(addText, Add),
                        new CliNodeRemove(removeText, Remove),
                        _content,
                    ];
        }

        _state.Data.MessageFilter.OnFilterAdded += (_, filter) => { 
                                                       _content.AddNode(FilterToNode(filter));
                                                   };
        
        _state.Data.MessageFilter.OnFilterRemoved += (_, index) => { 
                                                         _content.RemoveNode(index+2);
                                                   };
    }

    private void Add(Filter filter) {
        _addHandler.Invoke(filter);
    }

    private bool Remove(int index) {
        var filters = _messageFilter.GetFilters();

        if (index < 0 || index >= _content.Nodes.Count-2) {
            return false;
        }

        return !filters[index].IsDefault 
            && _removeHandler.Invoke(index);
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