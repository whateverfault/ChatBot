using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.interpreter.data;

public enum VariableType {
    String,
    Number,
    Bool,
}

public class Variable {
    private static readonly InterpreterService _interpreter = (InterpreterService)ServiceManager.GetService(ServiceName.Interpreter);
    
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("type")]
    public VariableType Type { get; private set; }
    
    [JsonProperty("value")]
    public string Value { get; private set; }


    [JsonConstructor]
    public Variable(
        [JsonProperty("name")] string name,
        [JsonProperty("type")] VariableType type,
        [JsonProperty("value")] string value) {
        Name = name;
        Type = type;
        Value = value;
    }

    public void SetName(string name) {
        Name = name;
        _interpreter.Options.Save();
    }

    public void SetType(VariableType type) {
        Type = type;
        _interpreter.Options.Save();
    }
    
    public void SetValue(string value, VariableType type) {
        _interpreter.SetVariable(Name, value, type);
    }
}