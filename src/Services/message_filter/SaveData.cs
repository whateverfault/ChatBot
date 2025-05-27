using System.Text.RegularExpressions;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.message_filter;

public class SaveData {
    public List<CommentedRegex> patterns;
    public State state;


    public SaveData(State state, List<CommentedRegex> patterns) {
        this.state = state;
        this.patterns = patterns;
    }
}