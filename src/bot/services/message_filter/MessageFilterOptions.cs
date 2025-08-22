using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.message_filter;

public class MessageFilterOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    private List<Filter> Filters => _saveData!.Filters;
    
    protected override string Name => "messageFilter";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.State;
    

    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
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
    
    public void AddFilter(Filter filter) {
        Filters.Add(filter);
        Save();
    }

    public bool RemoveFilter(int index) {
        if (index < 0 || index >= Filters.Count) return false;
        
        Filters.RemoveAt(index);
        Save();
        return true;
    }

    public List<Filter> GetFilters() {
        return Filters;
    }
}