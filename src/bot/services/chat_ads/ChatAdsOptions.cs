using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_ads.data;
using ChatBot.bot.services.chat_ads.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "chat_ads";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    private List<ChatAd> ChatAds => _saveData!.ChatAds;
    
    
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
        _saveData!.ServiceState = state;
        Save();
    }

    public void AddChatAd(ChatAd chatAd) {
        ChatAds.Add(chatAd);
        Save();
    }
    
    public bool RemoveChatAd(int index) {
        if (index < 0 || index >= ChatAds.Count) return false;
        
        ChatAds.RemoveAt(index);
        Save();
        return true;
    }

    public List<ChatAd> GetChatAds() {
        return ChatAds;
    }
}