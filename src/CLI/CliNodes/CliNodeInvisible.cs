namespace ChatBot.CLI.CliNodes;

public class CliNodeInvisible : CliNode {
    protected override string Text { get; }


    public override int PrintValue(int index, out string end) {
        end = "";
        return 1;
    }

    public override void Activate(CliState state) {}
}