using ChatBot.Services.interfaces;
using ChatBot.utils;
using TwitchLib.Client.Models;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    private readonly string _grSavePath = Path.Combine(Shared.saveDirectory, "game-requests.json");
    private readonly string _grpSavePath = Path.Combine(Shared.saveDirectory, "game-requests-points.json");
    private readonly string _stateSavePath = Path.Combine(Shared.saveDirectory, "game-requests-state.json");
    private List<GameRequest>? _gameRequests;
    private Dictionary<string, int>? _gameRequestsPoint;

    public override bool Enabled => _enabled;
    private bool _enabled;
    

    public override ErrorCode Enable(ChatMessage message) {
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        
        _enabled = true;
        JsonUtils.WriteSafe(_stateSavePath, Shared.saveDirectory, _enabled);
        return ErrorCode.None;
    }

    public override ErrorCode Disable(ChatMessage message) { 
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        
        _enabled = false;
        JsonUtils.WriteSafe(_stateSavePath, Shared.saveDirectory, _enabled);
        return ErrorCode.None;
    }

    public ErrorCode GivePoint(ChatMessage message, string? userId) {
        if (!Enabled) return ErrorCode.ServiceDisabled;
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;

        if (_gameRequestsPoint!.ContainsKey(userId!)) {
            _gameRequestsPoint[userId!] += 1;
        } else {
            _gameRequestsPoint.Add(userId!, 1);
        }
        
        return ErrorCode.None;
    }
    
    public ErrorCode TakePoint(ChatMessage message, string? userId) {
        if (!Enabled) return ErrorCode.ServiceDisabled;
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;

        if (_gameRequestsPoint!.ContainsKey(userId!)) {
            _gameRequestsPoint[userId!] -= _gameRequestsPoint[userId!] <= 0? 0 : 1;
        } else {
            return ErrorCode.WrongInput;
        }
        
        return ErrorCode.None;
    }
    
    public ErrorCode AppendRequest(GameRequest request, ChatMessage message) {
        if (!Enabled) return ErrorCode.ServiceDisabled;
        if (_gameRequestsPoint!.ContainsKey(message.UserId) && _gameRequestsPoint![message.UserId] <= 0) return ErrorCode.TooFewPoints;
        
        // TODO fix 'always contains' bug
        if (_gameRequests!.Contains(request)) return ErrorCode.AlreadyContains;
        
        _gameRequests?.Add(request);
        JsonUtils.WriteSafe(_grSavePath, Shared.saveDirectory, _gameRequests);
        
        return ErrorCode.None;
    }
    
    public ErrorCode RemoveRequestAt(int index, ChatMessage message) {
        if (!Enabled) return ErrorCode.ServiceDisabled;
        if (index >= _gameRequests?.Count) return ErrorCode.WrongInput;
        if (_gameRequestsPoint!.ContainsKey(message.UserId) && _gameRequestsPoint![message.UserId] <= 0) return ErrorCode.TooFewPoints;
        
        _gameRequests?.RemoveAt(index);
        JsonUtils.WriteSafe(_grSavePath, Shared.saveDirectory, _gameRequests);
        
        return ErrorCode.None;
    }

    public ErrorCode ListGameRequests(out GameRequest[] gameRequests) {
        gameRequests = [];
        if (!Enabled) return ErrorCode.ServiceDisabled;
        gameRequests =   _gameRequests!.ToArray();
        return ErrorCode.None;
    }

    public override void Init() {
        JsonUtils.TryRead(_grSavePath, out _gameRequests);
        JsonUtils.TryRead(_grpSavePath, out _gameRequestsPoint);
        JsonUtils.TryRead(_stateSavePath, out _enabled);
        _gameRequests ??= [];
        _gameRequestsPoint ??= [];
    }

    public override void Kill() {
        if (_gameRequests is not { Count: > 0 }) return;
        JsonUtils.WriteSafe(_grSavePath, Shared.saveDirectory, _gameRequests);
        JsonUtils.WriteSafe(_grpSavePath, Shared.saveDirectory, _gameRequestsPoint);
        JsonUtils.WriteSafe(_stateSavePath, Shared.saveDirectory, _enabled);
    }
}