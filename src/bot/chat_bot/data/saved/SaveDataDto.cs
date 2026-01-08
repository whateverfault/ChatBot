using ChatBot.api.basic;
using ChatBot.bot.services.scopes;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveDataDto {
    public readonly SafeField<ConnectionCredentials> Credentials = new SafeField<ConnectionCredentials>(new ConnectionCredentials());
    public readonly SafeField<ScopesPreset> CurAuthLevel = new SafeField<ScopesPreset>(ScopesPreset.None);
    public readonly SafeField<bool> HasBroadcasterAuth = new SafeField<bool>(false);

    
    public SaveDataDto() { }
    
    public SaveDataDto(ConnectionCredentials credentials,
                       ScopesPreset scopesPreset,
                       bool hasBroadAuth) {
        Credentials.Value = credentials;
        CurAuthLevel.Value = scopesPreset;
        HasBroadcasterAuth.Value = hasBroadAuth;
    }
}