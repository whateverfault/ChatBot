using Newtonsoft.Json;
using sonlanglib.interpreter.data.vars;

namespace ChatBot.bot.services.interpreter.data;

public class StoredVarValue {
    [JsonProperty("value")]
    public StoredSubValue Value { get; set; }
    
    [JsonProperty("type")]
    public VariableType Type { get; private set; }
    
    [JsonProperty("next")]
    public List<StoredVarValue> Next { get; private set; } 


    public StoredVarValue(VarValue value) {
        Type = value.Type;
        Value = new StoredSubValue(value.Value);

        Next = [];
        foreach (var val in value.Next) {
            var casted = new StoredVarValue(val);
            Next.Add(casted);
        }
    }
    
    [JsonConstructor]
    public StoredVarValue(
        [JsonProperty("value")] StoredSubValue value,
        [JsonProperty("type")] VariableType type,
        [JsonProperty("next")] List<StoredVarValue> next) {
        Value = value;
        Type = type;
        Next = next;
    }
}