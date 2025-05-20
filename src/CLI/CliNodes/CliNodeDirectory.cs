namespace ChatBot.CLI.CliNodes;

public class CliNodeDirectory : CliNode{
    public override string Text { get; }
    public override CliNodeType Type { get; }
    public CliNode[] nodes;

    public CliNodeDirectory( string text, CliNode[] nodes, ActionHandler? back = null) {
        Type = CliNodeType.Directory;
        
        Text = text;
        
        var nds = new List<CliNode>();
        if (back != null) { 
            nds.Add(new CliNodeAction("Back", back));
        }
        nds.AddRange(nodes);
        this.nodes = nds.ToArray();
    }
}