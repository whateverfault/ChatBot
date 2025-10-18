using Newtonsoft.Json;
using sonlanglib.interpreter.data.vars;

namespace ChatBot.bot.services.interpreter.data;

public class StoredSubValue {
    [JsonProperty("value")]
    public string Val { get; private set; }
    
    [JsonProperty("type")]
    public VariableType Type { get; private set; }

    
    public StoredSubValue(SubValue subValue) {
        Val = subValue.Val;
        Type = subValue.Type;
    }

    [JsonConstructor]
    public StoredSubValue(
        [JsonProperty("value")] string val,
        [JsonProperty("type")] VariableType type) {
        Val = val;
        Type = type;
    }
}