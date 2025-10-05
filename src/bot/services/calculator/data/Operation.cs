using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.calculator.data;

public enum OpScope {
    None = 0,
    Left = 1,
    Right = 2,
    LeftRight = 3,
}

public enum OpPriority {
    Lowest,
    Lower,
    Low,
    High,
    Highest,
}

public delegate Result<double?, ErrorCode?> EvaluationHandler(double a, double b);

public class Operation {
    public string Name { get; }
    public OpScope Scope { get; }
    public OpPriority Priority { get; }

    public EvaluationHandler Evaluation;
    

    public Operation(string name, EvaluationHandler evaluation, OpScope scope, OpPriority priority) {
        Name = name;
        Evaluation = evaluation;
        Scope = scope;
        Priority = priority;
    }
}