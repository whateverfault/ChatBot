using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    public override string Name => ServiceName.GameRequests;
    public override GameRequestsOptions Options { get; } = new();


    public override State GetServiceState() {
        return Options.State;
    }
    
    public override void ToggleService() {
        Options.SetState(Options.State == State.Enabled ? State.Disabled : State.Enabled);
    }

    public ErrorCode GivePoint(ChatMessage message, string? userId) {
        if (Options.State == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        if (!message.IsBroadcaster) {
            return ErrorCode.PermDeny;
        }

        if (!Options.GameRequestsPoint!.TryAdd(userId!, 1)) {
            Options.GameRequestsPoint[userId!] += 1;
        }

        Options.Save();
        return ErrorCode.None;
    }

    public ErrorCode TakePoint(ChatMessage message, string? userId) {
        if (Options.State == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }

        if (Options.GameRequestsPoint!.TryGetValue(userId!, out var value)) {
            Options.GameRequestsPoint[userId!] -= value <= 0 ? 0 : 1;
        } else {
            return ErrorCode.WrongInput;
        }

        Options.Save();
        return ErrorCode.None;
    }

    public ErrorCode AppendRequest(GameRequest request, ChatMessage message) {
        if (Options.State == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        if (Options.GameRequestsPoint!.TryGetValue(message.UserId, out var value) && value <= 0) {
            return ErrorCode.TooFewPoints;
        }
        if (Options.GameRequestsSet!.Contains(request.GameName.GetHashCode())) {
            return ErrorCode.AlreadyContains;
        }

        Options.GameRequests?.Add(request);
        Options.GameRequestsSet?.Add(request.GetHashCode());
        Options.Save();
        return ErrorCode.None;
    }

    public ErrorCode RemoveRequestAt(int index, ChatMessage message) {
        if (Options.State == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }
        if (index >= Options.GameRequests?.Count) {
            return ErrorCode.WrongInput;
        }

        Options.GameRequests?.RemoveAt(index);
        Options.Save();
        return ErrorCode.None;
    }

    public ErrorCode ListGameRequests(out GameRequest[] gameRequests) {
        gameRequests = [];
        if (Options.State == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        gameRequests = Options.GameRequests!.ToArray();
        return ErrorCode.None;
    }

    public override void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }
}