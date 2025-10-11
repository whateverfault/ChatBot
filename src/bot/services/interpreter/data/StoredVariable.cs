using Newtonsoft.Json;
using sonlanglib.interpreter.data;

namespace ChatBot.bot.services.interpreter.data;

public class StoredVariable {
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("type")]
    public VariableType Type { get; private set; }
    
    [JsonProperty("value")]
    public List<string> Values { get; private set; }


    public StoredVariable(Variable variable) {
        Name = variable.Name;
        Type = variable.Type;
        Values = variable.Values;
    }
    
    [JsonConstructor]
    public StoredVariable(
        [JsonProperty("name")] string name,
        [JsonProperty("type")] VariableType type,
        [JsonProperty("value")] List<string> values) {
        Name = name;
        Type = type;
        Values = values;
    }
}