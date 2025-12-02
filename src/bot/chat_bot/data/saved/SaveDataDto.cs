using ChatBot.api.basic;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot.data.saved;

internal sealed class SaveDataDto {
    public readonly SafeField<ConnectionCredentials> Credentials = new SafeField<ConnectionCredentials>(new ConnectionCredentials());


    public SaveDataDto() { }
    
    public SaveDataDto(ConnectionCredentials credentials) {
        Credentials.Value = credentials;
    }
}