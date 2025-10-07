namespace ChatBot.bot.services.interpreter.lexer;

public enum ExpressionTokenType {
    Name,
    Number,
    Bool,
    String,
    SpecialOperation,
    ArithmeticOperation,
    ClosingParenthesis,
    OpeningParenthesis,
    Semicolon,
}

public class ExpressionToken {
    public ExpressionTokenType Type;
    public string Value;


    public ExpressionToken(ExpressionTokenType type, string value) {
        Type = type;
        Value = value;
    }
}