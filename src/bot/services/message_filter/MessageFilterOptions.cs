using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.message_filter.data;
using ChatBot.bot.services.message_filter.data.saved;
using ChatBot.bot.shared;
using TwitchAPI.api.data.responses.GetUserInfo;

namespace ChatBot.bot.services.message_filter;

public class MessageFilterOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "messageFilter";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    private List<Filter> Filters => _saveData!.Filters;
    private List<UserInfo> BannedUsers => _saveData!.BannedUsers;
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
        if (index < 0 
         || index >= Filters.Count
         || Filters[index].IsDefault) return false;
        
        Filters.RemoveAt(index);
        Save();
        return true;
    }

    public List<Filter> GetFilters() {
        return Filters;
    }

    public void AddBannedUser(UserInfo userInfo) {
        BannedUsers.RemoveAll(x => x.UserId.Equals(userInfo.UserId));
        BannedUsers.Add(userInfo);
        Save();
    }

    public bool RemoveBannedUser(int index) {
        if (index < 0 || index >= BannedUsers.Count) return false;

        BannedUsers.RemoveAt(index);
        Save();
        return true;
    }
    
    public int RemoveBannedUser(UserInfo userInfo) {
        var index = BannedUsers.FindIndex(x => x.UserId.Equals(userInfo.UserId));
        if (!RemoveBannedUser(index)) 
            return -1;

        Save();
        return index;

    }
    
    public IReadOnlyList<UserInfo> GetBannedUsers() {
        return BannedUsers;
    }
}