using TwitchLib.Client.Models;

namespace ChatBot.Shared.Handlers;

public enum Permission {
    Dev,
    Broadcaster,
    Mods,
    Vips,
    OnlyVips,
    Everyone
}

public class PermissionHandler {
    public static bool Handle(Permission permission, ChatMessage message) {
        return permission switch {
                   Permission.Dev         => message.IsBroadcaster || message.UserId == Constants.DevUserId,
                   Permission.Broadcaster => message.IsBroadcaster,
                   Permission.Mods        => message.IsBroadcaster || message.IsModerator,
                   Permission.Vips        => message.IsBroadcaster || message.IsModerator || message.IsVip,
                   Permission.OnlyVips    => message.IsBroadcaster || message.IsVip,
                   Permission.Everyone    => true,
                   _                      => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
               };
    }
}