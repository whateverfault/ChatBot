namespace ChatBot.cli.data.CliNodes;

public class CliNodeInvisible : CliNode {
    public override string Text => string.Empty;


    public override int PrintValue(int index, out string end, bool drawIndex = false) {
        end = "";
        return 1;
    }

    public override void Activate(CliState state) {}
}