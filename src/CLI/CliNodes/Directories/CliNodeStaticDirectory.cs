namespace ChatBot.cli.CliNodes.Directories;

public delegate string DynamicName();

public sealed class CliNodeStaticDirectory : CliNodeDirectory {
    private readonly DynamicName? _dynamicName;
    private readonly bool _hasBackOption;

    private readonly string _text;

    protected override string Text => 
        _dynamicName != null?
            _dynamicName.Invoke() :
            _text;
    
    public override List<CliNode> Nodes { get; }


    public CliNodeStaticDirectory(string text, CliState state, bool hasBackOption, CliNode[] nodes, DynamicName? dynamicName = null) {
        _text = text;
        Nodes = []; 
        _hasBackOption = hasBackOption;
        _dynamicName = dynamicName;

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
}