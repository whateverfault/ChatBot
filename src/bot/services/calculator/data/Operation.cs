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
}

public delegate Result<double?, ErrorCode?> EvaluationHandler(double a, double b);

public class Operation {
    public OpScope Scope { get; }
    public OpPriority Priority { get; }

    public EvaluationHandler Evaluation;
    

    public Operation(EvaluationHandler evaluation, OpScope scope, OpPriority priority) {
        Evaluation = evaluation;
        Scope = scope;
        Priority = priority;
    }
}