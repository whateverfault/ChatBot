namespace ChatBot.cli.data.CliNodes.Directories;

public delegate string DynamicName();

public sealed class CliNodeStaticDirectory : CliNodeDirectory {
    private readonly DynamicName? _dynamicName;

    private readonly string _text;

    protected override string Text => 
        _dynamicName != null?
            _dynamicName.Invoke() :
            _text;
    
    public override List<CliNode> Nodes { get; }
    public override bool HasBackOption { get; }


    public CliNodeStaticDirectory(string text, CliState state, bool hasBackOption, CliNode[] nodes, DynamicName? dynamicName = null) {
        _text = text;
        Nodes = []; 
        HasBackOption = hasBackOption;
        _dynamicName = dynamicName;

        if (state.NodeSystem != null) {
            if (hasBackOption) {
                Nodes.Add(new CliNodeAction("Back", state.NodeSystem.DirectoryBack));
            } else {
                Nodes.Add(new CliNodeInvisible());
            }
        }
        
        Nodes.AddRange(nodes);
    }

    public override void Activate(CliState state) {
        state.NodeSystem?.DirectoryEnter(this);
    }

    public void AddNode(CliNode node) {
        Nodes.Add(node);
    }

    public void AddNodeRange(CliNode[] nodes) {
        Nodes.AddRange(nodes);
    }
    
    public void RemoveNode(int index) {
        if (HasBackOption && (index < 1 || index >= Nodes.Count)) return;
        
        Nodes.RemoveAt(index);
    }
}