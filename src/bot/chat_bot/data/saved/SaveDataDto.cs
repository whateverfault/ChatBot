using ChatBot.api.basic;
using ChatBot.bot.services.scopes;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveDataDto {
    public readonly SafeField<ConnectionCredentials> Credentials = new SafeField<ConnectionCredentials>(new ConnectionCredentials());
    public readonly SafeField<AuthLevel> AuthenticationLevel = new SafeField<AuthLevel>(AuthLevel.None);
    public readonly SafeField<FullCredentials?> AuthorizedCredentials = new SafeField<FullCredentials?>(null);

    
    public SaveDataDto() { }
    
    public SaveDataDto(ConnectionCredentials credentials,
                       AuthLevel authLevel,
                       FullCredentials? authedCreds) {
        Credentials.Value = credentials;
        AuthenticationLevel.Value = authLevel;
        AuthorizedCredentials.Value = authedCreds;
    }
}