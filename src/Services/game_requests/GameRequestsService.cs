using ChatBot.Services.interfaces;
using ChatBot.utils;
using TwitchLib.Client.Models;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    private readonly string _savePath = Path.Combine(Shared.saveDirectory, "game_requests.json");
    private List<GameRequest>? _gameRequests;

    public override bool Enabled => _enabled;
    private bool _enabled;
    

    public override ErrorCode Enable(ChatMessage message) {
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        
        _enabled = true;
        return ErrorCode.None;
    }

    public override ErrorCode Disable() {
        _enabled = false;
        return ErrorCode.None;
    }
    public ErrorCode AppendRequest(GameRequest request, ChatMessage message) {
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        if (_gameRequests!.Contains(request)) return ErrorCode.AlreadyContains;
        
        _gameRequests?.Add(request);
        JsonUtils.WriteSafe(_savePath, Shared.saveDirectory, _gameRequests);
        
        return ErrorCode.None;
    }
    
    public ErrorCode RemoveRequestAt(int index, ChatMessage message) {
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        if (index >= _gameRequests?.Count) return ErrorCode.WrongInput;
        
        _gameRequests?.RemoveAt(index);
        JsonUtils.WriteSafe(_savePath, Shared.saveDirectory, _gameRequests);
        
        return ErrorCode.None;
    }

    public GameRequest[] GetGameRequests() {
        return _gameRequests!.ToArray();
    }

    public override void Init() {
        JsonUtils.TryRead(_savePath, out _gameRequests);
        _gameRequests ??= [];
    }

    public override void Kill() {
        if (_gameRequests is not { Count: > 0 }) return;
        JsonUtils.WriteSafe(_savePath, Shared.saveDirectory, _gameRequests);
    }
}