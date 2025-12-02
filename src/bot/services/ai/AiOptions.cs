using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai.data;
using ChatBot.bot.services.ai.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.ai;

public class AiOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private static string Name => "ai";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public List<AiData> AiData => _saveData!.AiData;
    public string GoogleProjectId => _saveData!.GoogleProjectId;
    public AiKind AiKind => _saveData!.AiKind;

    public long RemoveChatAfter => _saveData!.RemoveChatIn;
    
    public List<AiChatHistory> Chats = [];
    
    
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
    
    public void SetAiKind(AiKind kind) {
        _saveData!.AiKind = kind;
        Save();
    }

    public void SetGoogleProjectId(string projectId) {
        _saveData!.GoogleProjectId = projectId;
        Save();
    }

    public AiChatHistory CreateChat() {
        var generatedId = Random.Shared.NextInt64(0, 65536);
        var id = generatedId.ToString("X4");
        RemoveChat(id);

        var chat = new AiChatHistory(id);
        Chats.Add(chat);
        return chat;
    }
    
    public AiChatHistory? GetChat(string? id) {
        if (string.IsNullOrEmpty(id)) return null;
        return Chats.FirstOrDefault(ch => ch.Id.Equals(id));
    }
    
    public void RemoveChat(string id) {
        for (var i = 0; i < Chats.Count; ++i) {
            if (!Chats[i].Id.Equals(id)) continue;
            
            Chats.RemoveAt(i);
            break;
        }
    }

    public void SetRemoveChatIn(long value) {
        _saveData!.RemoveChatIn = value;
        Save();
    }
}