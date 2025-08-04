namespace ChatBot.cli.CliNodes;

public class CliNodeInvisible : CliNode {
    protected override string Text => string.Empty;


    public override int PrintValue(int index, out string end) {
        end = "";
        return 1;
    }

    public override void Activate(CliState state) {}
}