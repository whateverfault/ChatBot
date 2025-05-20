namespace ChatBot.CLI.CliNodes;

public class CliNodeToggle : CliNode {

    public override CliNodeType Type { get; }
    public override string Text { get; }
    
    public bool Value { get; set; }


    public CliNodeToggle(string text, ref bool value) {
        Type = CliNodeType.Toggle;

        Text = text;
        Value = value;
    }
}