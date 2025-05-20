using ChatBot.Services.interfaces;
using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    private List<GameRequest>? _gameRequests;
    private Dictionary<string, int>? _gameRequestsPoint;

    public override GameRequestsOptions Options { get; } = new();

    
    public override ErrorCode Enable(ChatMessage message) {
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        
        Options.SetState(State.Enabled);
        Options.Save();
        return ErrorCode.None;
    }

    public override ErrorCode Disable(ChatMessage message) { 
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;
        
        Options.SetState(State.Disabled);
        Options.Save();
        return ErrorCode.None;
    }

    public override State GetServiceState() {
        return Options.ServiceState;
    }
    
    public override void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled? State.Disabled : State.Enabled);
    }
    
    public ErrorCode GivePoint(ChatMessage message, string? userId) {
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;

        if (_gameRequestsPoint!.ContainsKey(userId!)) {
            _gameRequestsPoint[userId!] += 1;
        } else {
            _gameRequestsPoint.Add(userId!, 1);
        }
        
        return ErrorCode.None;
    }
    
    public ErrorCode TakePoint(ChatMessage message, string? userId) {
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        if (!message.IsBroadcaster) return ErrorCode.PermDeny;

        if (_gameRequestsPoint!.ContainsKey(userId!)) {
            _gameRequestsPoint[userId!] -= _gameRequestsPoint[userId!] <= 0? 0 : 1;
        } else {
            return ErrorCode.WrongInput;
        }
        
        return ErrorCode.None;
    }
    
    public ErrorCode AppendRequest(GameRequest request, ChatMessage message) {
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        if (_gameRequestsPoint!.TryGetValue(message.UserId, out var value) && value <= 0) return ErrorCode.TooFewPoints;
        
        // TODO fix 'always contains' bug
        if (_gameRequests!.Contains(request)) return ErrorCode.AlreadyContains;
        
        _gameRequests?.Add(request);
        Options.Save();
        return ErrorCode.None;
    }
    
    public ErrorCode RemoveRequestAt(int index, ChatMessage message) {
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        if (index >= _gameRequests?.Count) return ErrorCode.WrongInput;
        if (_gameRequestsPoint!.ContainsKey(message.UserId) && _gameRequestsPoint![message.UserId] <= 0) return ErrorCode.TooFewPoints;
        
        _gameRequests?.RemoveAt(index);
        Options.Save();
        return ErrorCode.None;
    }

    public ErrorCode ListGameRequests(out GameRequest[] gameRequests) {
        gameRequests = [];
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        gameRequests = _gameRequests!.ToArray();
        return ErrorCode.None;
    }

    public override void Init() {
        Options.Load();
        _gameRequests ??= [];
        _gameRequestsPoint ??= [];
    }
}