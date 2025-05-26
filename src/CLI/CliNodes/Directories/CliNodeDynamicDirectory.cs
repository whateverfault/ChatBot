namespace ChatBot.CLI.CliNodes.Directories;

public class CliNodeDynamicDirectory : CliNodeDirectory {
    private readonly AddHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    private readonly CliNodeStaticDirectory _dynamicDir;
    
    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }


    public CliNodeDynamicDirectory(
        string text,
        string addText,
        string removeText,
        AddHandler addHandler,
        RemoveHandler removeHandler,
        List<string> nodesContent,
        CliState state) {
        Text = text;
        _addHandler = addHandler;
        _removeHandler = removeHandler;

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
                                            3
                                            )
                            );
        _dynamicDir.AddNodeRange(
                                 nodesContent
                                 .Select(content => new CliNodeText(content))
                                 .ToArray<CliNode>()
                                 );

        Nodes = [
                    new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                    new CliNodeAdd(addText, Add),
                    new CliNodeRemove(removeText, Remove),
                    _dynamicDir,
                ];
    }
    
    public override void Activate(CliState state) {
        state.NodeSystem.DirectoryEnter(this);
    }

    private void Add(string value) {
        _dynamicDir.AddNode(new CliNodeText(value));
        _addHandler.Invoke(value);
    }
    
    private void Remove(int index) {
        if (index < 0 || index >= _dynamicDir.Nodes.Count-2) {
            return;
        }
        
        _dynamicDir.RemoveNode(index+2);
        _removeHandler.Invoke(index);
    }
}