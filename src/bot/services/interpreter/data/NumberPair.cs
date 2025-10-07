namespace ChatBot.bot.services.interpreter.data;

public class NumberPair {
    public double? Left { get; set; }
    public double? Right { get; set; }


    public NumberPair(double? left, double? right) {
        Left = left;
        Right = right;
    }
}