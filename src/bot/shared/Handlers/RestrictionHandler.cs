using ChatBot.api.twitch.client.data;

namespace ChatBot.bot.shared.handlers;

public enum Restriction {
    DevOnly,
    Broadcaster,
    DevBroad,
    DevMod,
    VipOnly,
    Mod,
    Vip,
    Everyone,
}

public static class RestrictionHandler {
    public static bool Fits(this ChatMessage message, Restriction restriction) {
        return restriction switch {
                   Restriction.DevOnly     => message.UserId == Constants.DevUserId,
                   Restriction.Broadcaster => message.IsBroadcaster,
                   Restriction.DevBroad    => message.IsBroadcaster || message.UserId == Constants.DevUserId,
                   Restriction.DevMod      => message.IsBroadcaster || message.IsModerator || message.UserId == Constants.DevUserId,
                   Restriction.VipOnly     => message.IsBroadcaster || message.IsVip,
                   Restriction.Mod         => message.IsBroadcaster || message.IsModerator,
                   Restriction.Vip         => message.IsBroadcaster || message.IsModerator || message.IsVip,
                   Restriction.Everyone    => true,
                   _                       => throw new ArgumentOutOfRangeException(nameof(restriction), restriction, null),
               };
    }
}