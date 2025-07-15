using System.Text.RegularExpressions;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.message_filter;

public class MessageFilterOptions : Options {
    private SaveData? _saveData;
    private List<Filter> Filters => _saveData!.Filters;
    
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
                                 [
                                     new Filter(
                                                "Level Requests",
                                                @"(?:^|\s)(\d{8,11})",
                                                true
                                                ),
                                     new Filter(
                                                "Special Symbols",
                                                "^[!@~]+",
                                                true
                                                ),
                                 ],
                                 State.Disabled
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

    public List<Filter> GetFilters() {
        return Filters;
    }
    
    public void AddFilter(Filter filter) {
        Filters.Add(filter);
        Save();
    }

    public void RemovePattern(int index) {
        Filters.RemoveAt(index);
        Save();
    }
}