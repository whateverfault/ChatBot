using TwitchLib.Client.Models;

namespace ChatBot.shared.Handlers;

public enum Restriction {
    DevBroad,
    DevMod,
    DevOnly,
    Broadcaster,
    Mod,
    Vip,
    VipOnly,
    Everyone
}

public static class RestrictionHandler {
    public static bool Handle(Restriction restriction, ChatMessage message) {
        return restriction switch {
                   Restriction.DevBroad         => message.IsBroadcaster || message.UserId == Constants.DevUserId,
                   Restriction.DevMod      => message.IsBroadcaster || message.IsModerator || message.UserId == Constants.DevUserId,
                   Restriction.DevOnly     => message.UserId == Constants.DevUserId,
                   Restriction.Broadcaster => message.IsBroadcaster,
                   Restriction.Mod         => message.IsBroadcaster || message.IsModerator,
                   Restriction.Vip         => message.IsBroadcaster || message.IsModerator || message.IsVip,
                   Restriction.VipOnly     => message.IsBroadcaster || message.IsVip,
                   Restriction.Everyone    => true,
                   _                       => throw new ArgumentOutOfRangeException(nameof(restriction), restriction, null)
               };
    }
}