using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ChatBot.Services.message_filter;

public class Filter {
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("pattern")]
    public string Pattern { get; private set; }
    
    [JsonProperty("is_default")]
    public bool IsDefault { get; private set; }
    
    
    [JsonConstructor]
    public Filter(
        [JsonProperty("name")] string name,
        [JsonProperty("pattern")] string pattern,
        [JsonProperty("is_default")] bool isDefault = false) {
        Name = name;
        Pattern = pattern;
        IsDefault = isDefault;
    }

    public string GetName() {
        return Name;
    }

    public void SetName(string name) {
        Name = name;
    }

    public string GetPattern() {
        return Pattern;
    }

    public void SetPattern(string pattern) {
        Pattern = pattern;
    }
}