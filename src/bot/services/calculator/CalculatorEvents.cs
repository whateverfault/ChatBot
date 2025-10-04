using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.calculator;

public class CalculatorEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}