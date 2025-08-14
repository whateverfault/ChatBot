using ChatBot.bot.services.chat_ads.Data;
using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;
using ChatBot.bot.utils;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "chat_ads";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public List<ChatAd> ChatAds => _saveData!.ChatAds;
    
    
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