using Newtonsoft.Json;
using sonlanglib.interpreter.data.vars;

namespace ChatBot.bot.services.interpreter.data;

public class StoredVariable {
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("type")]
    public VariableType Type { get; private set; }
    
    [JsonProperty("value")]
    public StoredVarValue Value { get; private set; }


    public StoredVariable(Variable variable) {
        Name = variable.Name;
        Type = variable.Type;
        Value = new StoredVarValue(variable.Value);
    }
    
    [JsonConstructor]
    public StoredVariable(
        [JsonProperty("name")] string name,
        [JsonProperty("type")] VariableType type,
        [JsonProperty("value")] StoredVarValue value) {
        Name = name;
        Type = type;
        Value = value;
    }
}