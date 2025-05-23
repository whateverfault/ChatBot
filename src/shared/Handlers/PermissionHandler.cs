using TwitchLib.Client.Models;

namespace ChatBot.shared.Handlers;

public enum Permission {
    Dev,
    DevOnly,
    Broadcaster,
    Mod,
    Vip,
    VipOnly,
    Everyone
}

public static class PermissionHandler {
    public static bool Handle(Permission permission, ChatMessage message) {
        return permission switch {
                   Permission.Dev         => message.IsBroadcaster || message.UserId == Constants.DevUserId,
                   Permission.DevOnly     => message.UserId == Constants.DevUserId,
                   Permission.Broadcaster => message.IsBroadcaster,
                   Permission.Mod         => message.IsBroadcaster || message.IsModerator,
                   Permission.Vip         => message.IsBroadcaster || message.IsModerator || message.IsVip,
                   Permission.VipOnly     => message.IsBroadcaster || message.IsVip,
                   Permission.Everyone    => true,
                   _                      => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
               };
    }
}