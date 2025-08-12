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
        AddFilterHandler addHandler,
        RemoveHandler removeHandler,
        CliState state,
        List<Filter> filters) {
        _addHandler = addHandler;
        _removeHandler = removeHandler;
        _state = state;
        Text = text;

        
        _content = new CliNodeStaticDirectory(
                                              "Content",
                                              _state,
                                              true,
                                              []
                                              );
        
        _messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        
        foreach (var filter in filters) {
            _content.AddNode(FilterToNode(filter));
        }

        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeFilterAdd($"Add {text}", Add),
                    new CliNodeRemove($"Remove {text}", Remove),
                    _content,
                ];
    }

    private void Add(Filter filter) {
        _content.AddNode(FilterToNode(filter));
        _addHandler.Invoke(filter);
    }

    private bool Remove(int index) {
        var filters = _messageFilter.GetFilters();

        if (index < 0 || index > filters.Count || index >= _content.Nodes.Count-2) {
            return false;
        }

        if (filters[index].IsDefault) {
            return false;
        }
        
        _content.RemoveNode(index+2);
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