namespace ChatBot.cli.data.CliNodes.Directories;

public abstract class CliNodeDirectory : CliNode {
    public override string Text => string.Empty;

    public abstract List<CliNode> Nodes { get; }
    public abstract bool HasBackOption { get; }
    
    
    public override void Activate(CliState state) {
        state.NodeSystem?.DirectoryEnter(this);
    }
}