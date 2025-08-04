namespace ChatBot.cli.CliNodes.Directories;

public class CliNodeDynamicDirectory : CliNodeDirectory {
    private readonly bool _commented;
    private readonly AddWithCommentHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    private readonly CliNodeStaticDirectory _dynamicDir;
    private readonly CliState _state;
    
    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }


    public CliNodeDynamicDirectory(
        string text,
        string addText,
        string removeText,
        AddWithCommentHandler addHandler,
        RemoveHandler removeHandler,
        List<Content> nodesContent,
        CliState state,
        bool commented) {
        Text = text;
        _addHandler = addHandler;
        _removeHandler = removeHandler;
        _state = state;
        _commented = commented;

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
        foreach (var nodeContent in nodesContent) {
            if (nodeContent.HasComment) {
                _dynamicDir.AddNode(
                                    new CliNodeStaticDirectory
                                        (
                                         nodeContent.Comment,
                                         _state,
                                         true,
                                         [
                                             new CliNodeText(nodeContent.ContentString),
                                         ]
                                         )
                                    );
            } else {
                _dynamicDir.AddNode(
                                    new CliNodeText(nodeContent.ContentString)
                                    );
            }
        }

        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeAdd(addText, Add),
                    new CliNodeRemove(removeText, Remove),
                    _dynamicDir,
                ];
    }
    
    private void Add(string value) {
        if (!_commented) {
            _dynamicDir.AddNode(new CliNodeText(value));
            _addHandler.Invoke(value, false);
            return;
        }
        
        Console.Write("Comment: ");
        var comment = Console.ReadLine() ?? "";

        var commentedNode = new CliNodeStaticDirectory(
                                                       comment,
                                                       _state,
                                                       true,
                                                       [
                                                           new CliNodeText(value),
                                                       ]
                                                      );
        _dynamicDir.AddNode(commentedNode);
        _addHandler.Invoke(value, true, comment);
    }
    
    private bool Remove(int index) {
        if (index < 0 || index >= _dynamicDir.Nodes.Count-2) {
            return false;
        }
        
        _dynamicDir.RemoveNode(index+2);
        return _removeHandler.Invoke(index);
    }
}