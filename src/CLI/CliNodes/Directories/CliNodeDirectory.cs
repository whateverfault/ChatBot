namespace ChatBot.CLI.CliNodes.Directories;

public abstract class CliNodeDirectory : CliNode {
    protected override string Text { get; }

    public abstract List<CliNode> Nodes { get; }
    
    
    public override void Activate(CliState state) {
        throw new NotImplementedException();
    }
}