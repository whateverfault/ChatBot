using System.Text.RegularExpressions;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.message_filter;

public class MessageFilterOptions : Options {
    private SaveData? _saveData;
    private List<CommentedRegex> Patterns => _saveData!.Patterns;
    
    protected override string Name => "messageFilter";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.State;


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
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData(
                                 State.Disabled,
                                 [
                                     new CommentedRegex(
                                                        new Regex(@"(?:^|\s)(\d{8,11})(?=\s|$)"), 
                                                        true, 
                                                        "Level Requests"),
                                     new CommentedRegex(
                                                        new Regex(@"^[!@~]+"),
                                                        true,
                                                        "Special Symbols"
                                                        ),
                                     new CommentedRegex(
                                                        new Regex(@"^~"),
                                                        true,
                                                        "Commands"
                                                       ),
                                 ]
                                );
        Save();
    }

    public override void SetState(State state) {
        _saveData!.State = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }
    
    public void AddPattern(CommentedRegex regex) {
        Patterns.Add(regex);
        Save();
    }
    
    public void AddPatternWithComment(CommentedRegex regex) {
        Patterns.Add(regex);
        Save();
    }

    public void RemovePattern(int index) {
        Patterns.RemoveAt(index);
        Save();
    }

    public List<CommentedRegex> GetPatterns() {
        return Patterns;
    }
}