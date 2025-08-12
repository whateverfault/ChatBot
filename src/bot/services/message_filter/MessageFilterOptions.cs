using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot.services.message_filter;

public class MessageFilterOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    private List<Filter> Filters => _saveData!.Filters;
    
    protected override string Name => "messageFilter";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.State;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.State = state;
        Save();
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