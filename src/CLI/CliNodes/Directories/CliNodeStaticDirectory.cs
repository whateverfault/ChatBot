namespace ChatBot.CLI.CliNodes.Directories;

public class CliNodeStaticDirectory : CliNodeDirectory {
    private readonly bool _hasBackOption;

    protected override string Text { get; }

    public override List<CliNode> Nodes { get; }

    
    public CliNodeStaticDirectory(string text, CliState state, bool hasBackOption, CliNode[] nodes) {
        Text = text;
        Nodes = []; 
        _hasBackOption = hasBackOption;
        
        if (hasBackOption) {
            Nodes.Add(new CliNodeAction("Back", state.NodeSystem.DirectoryBack));
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