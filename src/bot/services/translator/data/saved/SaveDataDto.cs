using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.translator.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<string> ProjectId = new SafeField<string>(string.Empty);

    public readonly SafeField<string> Location = new SafeField<string>("global");

    public readonly SafeField<string> GoogleToken = new SafeField<string>(string.Empty);

    public readonly SafeField<string> VkToken = new SafeField<string>(string.Empty);

    public readonly SafeField<string> TargetLanguage = new SafeField<string>("ru");

    public readonly SafeField<TranslationService> TranslationService =
        new SafeField<TranslationService>(translator.TranslationService.Google);


    public SaveDataDto(){}
    
    public SaveDataDto(
        State serviceState,
        string projectId,
        string location,
        string googleToken,
        string vkToken,
        string targetLanguage,
        TranslationService service) {
        ServiceState.Value = serviceState;
        ProjectId.Value = projectId;
        Location.Value = location;
        GoogleToken.Value = googleToken;
        VkToken.Value = vkToken;
        TargetLanguage.Value = targetLanguage;
        TranslationService.Value = service;
    }
}