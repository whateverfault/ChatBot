namespace ChatBot.cli.CliNodes.Directories;

public abstract class CliNodeDirectory : CliNode {
    protected override string Text { get; } = null!;

    public abstract List<CliNode> Nodes { get; }
    
    
    public override void Activate(CliState state) {
        state.NodeSystem.DirectoryEnter(this);
    }
}