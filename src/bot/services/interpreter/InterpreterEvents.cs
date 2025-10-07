using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.interpreter;

public class InterpreterEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}