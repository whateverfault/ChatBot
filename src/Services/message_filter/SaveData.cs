using System.Text.RegularExpressions;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.message_filter;

public class SaveData {
    [JsonProperty(PropertyName ="patterns")]
    public List<CommentedRegex> Patterns { get; }
    [JsonProperty(PropertyName ="state")]
    public State State { get; set; }


    public SaveData(State state, List<CommentedRegex> patterns) {
        State = state;
        Patterns = patterns;
    }
}