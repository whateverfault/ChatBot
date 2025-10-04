namespace ChatBot.bot.services.calculator.data;

public enum ExprNodeKind{
    Op,
    Num,
}

public class ExprNode {
    public ExprNodeKind Kind { get; private set; }
    public string Value { get; set; }


    public ExprNode(string value, ExprNodeKind kind) {
        Value = value;
        Kind = kind;
    }

    public void ChangeKind(ExprNodeKind kind) {
        Kind = kind;
    }
}