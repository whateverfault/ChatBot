namespace ChatBot.CLI.CliNodes.Directories;

public delegate string NameUpdateHandler(int index);

public sealed class CliNodeStaticDirectory : CliNodeDirectory {
    private readonly NameUpdateHandler? _nameUpdater;
    private readonly bool _hasBackOption;
    private string _text;

    protected override string Text => _text;

    public override List<CliNode> Nodes { get; }


    public CliNodeStaticDirectory(string text, CliState state, bool hasBackOption, CliNode[] nodes, NameUpdateHandler? nameUpdater = null) {
        _text = text;
        Nodes = []; 
        _hasBackOption = hasBackOption;
        _nameUpdater = nameUpdater;

        if (hasBackOption) {
            Nodes.Add(new CliNodeAction("Back", state.NodeSystem.DirectoryBack));
        } else {
            Nodes.Add(new CliNodeInvisible());
        }
        if (nodes.Length <= 0) return;
        
        Nodes.AddRange(nodes);
    }

    public override void Activate(CliState state) {
        state.NodeSystem.DirectoryEnter(this);
    }

    public void AddNode(CliNode node) {
        Nodes.Add(node);
    }

    public void AddNodeRange(CliNode[] nodes) {
        Nodes.AddRange(nodes);
    }
    
    public void RemoveNode(int index) {
        if (_hasBackOption && (index < 1 || index >= Nodes.Count)) return;
        
        Nodes.RemoveAt(index);
    }

    public void UpdateName(int index) {
        _nameUpdater!.Invoke(index);
    }
}