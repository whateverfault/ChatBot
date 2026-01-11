using System.Text;
using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.scopes;

public enum Scope {
    Invalid,
    ClipsEdit,
    ChannelManageBroadcast,
    ChatRead,
    ChatEdit,
    ChannelModerate,
    ChannelReadRedemptions,
    ChannelManageRedemptions,
    ModeratorManageBannedUsers,
    ModeratorManageChatMessages,
    ModeratorReadFollowers,
    ChannelBot,
    UserBot,
    UserReadChat,
    UserWriteChat,
}

public enum ScopesPreset {
    None = 0,
    Bot,
    Broadcaster,
}

public class ScopesService : Service {
    private static readonly List<Scope>[] _scopesPresets = [
                                                               [],
                                                               [
                                                                   Scope.ChatRead, Scope.ChatEdit, Scope.UserBot, 
                                                                   Scope.ChannelBot, Scope.ClipsEdit, Scope.UserReadChat,
                                                                   Scope.UserWriteChat, Scope.ChannelModerate, Scope.ModeratorManageBannedUsers,
                                                                   Scope.ModeratorManageChatMessages, Scope.ModeratorReadFollowers, 
                                                               ],
                                                               [
                                                                   Scope.ChannelManageBroadcast, Scope.ChannelManageRedemptions, Scope.ChannelReadRedemptions,
                                                               ],
                                                           ]; 
    
    public override ScopesOptions Options { get; } = new ScopesOptions();


    public string GetScopesString() {
        if (Options.Preset is ScopesPreset.None) {
            return "none";
        }
        
        return GetScopesString(_scopesPresets[(int)Options.Preset]);
    }
    
    public void ScopesPresetNext() {
        var preset = (ScopesPreset)(((int)Options.Preset + 1) % Enum.GetValues(typeof(ScopesPreset)).Length);
        if (preset == ScopesPreset.None) {
            preset = ScopesPreset.Bot;
        }
        
        Options.SetPreset(preset);
    }

    public int GetScopesPresetAsInt() {
        return (int)Options.Preset;
    }
    
    public ScopesPreset GetScopesPreset() {
        return Options.Preset;
    }
    
    private string GetScopesString(List<Scope> scopes) {
        var sb = new StringBuilder();

        for (var i = 0; i < scopes.Count; i++) {
            sb.Append(GetScopeString(scopes[i]));

            if (i < scopes.Count - 1) {
                sb.Append('+');
            }
        }

        return sb.ToString();
    }
    
    private string GetScopeString(Scope scope) {
        return scope switch {
            Scope.ClipsEdit                   => "clips:edit",
            Scope.ChannelManageBroadcast      => "channel:manage:broadcast",
            Scope.ChatRead                    => "chat:read",
            Scope.ChatEdit                    => "chat:edit",
            Scope.ChannelModerate             => "channel:moderate",
            Scope.ChannelReadRedemptions      => "channel:read:redemptions",
            Scope.ChannelManageRedemptions    => "channel:manage:redemptions",
            Scope.ModeratorManageBannedUsers  => "moderator:manage:banned_users",
            Scope.ModeratorManageChatMessages => "moderator:manage:chat_messages",
            Scope.ModeratorReadFollowers      => "moderator:read:followers",
            Scope.ChannelBot                  => "channel:bot",
            Scope.UserBot                     => "user:bot",
            Scope.UserReadChat                => "user:read:chat",
            Scope.UserWriteChat               => "user:write:chat",
            
            _ => string.Empty,
        };
    }

    public ScopesPreset GetScopesPreset(string scopesRaw) {
        var scopes = ParseScopes(scopesRaw);

        if (scopes.Count == _scopesPresets[(int)ScopesPreset.Bot].Count) {
            return ScopesPreset.Bot;
        }

        return ScopesPreset.Broadcaster;
    }
    
    private List<Scope> ParseScopes(string scopesRaw) {
        var scopes = new List<Scope>();

        var separated = scopesRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var scope in separated) {
            scopes.Add(ParseScope(scope));
        }
        
        return scopes;
    }

    private Scope ParseScope(string scope) {
        return scope switch {
            "clips:edit"                     => Scope.ClipsEdit,
            "channel:manage:broadcast"       => Scope.ChannelManageBroadcast,
            "chat:read"                      => Scope.ChatRead,
            "chat:edit"                      => Scope.ChatEdit,
            "channel:moderate"               => Scope.ChannelModerate,
            "channel:read:redemptions"       => Scope.ChannelReadRedemptions,
            "channel:manage:redemptions"     => Scope.ChannelManageRedemptions,
            "moderator:manage:banned_users"  => Scope.ModeratorManageBannedUsers,
            "moderator:manage:chat_messages" => Scope.ModeratorManageChatMessages,
            "moderator:read:followers"       => Scope.ModeratorReadFollowers,
            "channel:bot"                    => Scope.ChannelBot,
            "user:bot"                       => Scope.UserBot,
            "user:read:chat"                 => Scope.UserReadChat,
            "user:write:chat"                => Scope.UserWriteChat,
            
            _ => Scope.Invalid,
        };
    }
}