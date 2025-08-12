using ChatBot.bot.services.Static;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.message_filter;

public class Filter {
    private static readonly Options _messageFilter = ServiceManager.GetService(ServiceName.MessageFilter).Options;
    
    
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
        _messageFilter.Save();
    }

    public string GetPattern() {
        return Pattern;
    }

    public void SetPattern(string pattern) {
        Pattern = pattern;
        _messageFilter.Save();
    }
}