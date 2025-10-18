using ChatBot.bot.services.interfaces;
using sonlanglib.interpreter.data.vars;

namespace ChatBot.bot.services.interpreter;

public class InterpreterEvents : ServiceEvents {
    private InterpreterService? _interpreter;
    
    public override bool Initialized { get; protected set; }


    public override void Init(Service service) {
        _interpreter = (InterpreterService)service;
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        if (_interpreter == null) return;
        _interpreter.Interpreter.OnVariableChanged += SaveVariableWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        if (_interpreter == null) return;
        _interpreter.Interpreter.OnVariableChanged -= SaveVariableWrapper;
    }

    private void SaveVariableWrapper(object? sender, Variable var) {
        _interpreter?.Options.SaveVariable(var);
    }
}