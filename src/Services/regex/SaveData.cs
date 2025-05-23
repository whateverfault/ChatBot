using System.Text.RegularExpressions;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.regex;

public class SaveData {
    public List<Regex> patterns;
    public State state;


    public SaveData(State state, List<Regex> patterns) {
        this.state = state;
        this.patterns = patterns;
    }
}