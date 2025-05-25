using System.Text.RegularExpressions;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.regex;

public class RegexOptions : Options {
    private SaveData? _saveData;
    private List<Regex> Patterns => _saveData!.patterns;

    protected override string Name => "regex";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");

    public override State State => _saveData!.state;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData(
                                 State.Disabled,
                                 []
                                );
        Save();
    }

    public override void SetState(State state) {
        _saveData!.state = state;
        Save();
    }

    public override State GetState() {
        return State;
    }
    
    public void AddPattern(Regex regex) {
        Patterns.Add(regex);
        Save();
    }

    public void RemovePattern(int index) {
        Patterns.RemoveAt(index);
        Save();
    }

    public List<Regex> GetPatterns() {
        return Patterns;
    }
}