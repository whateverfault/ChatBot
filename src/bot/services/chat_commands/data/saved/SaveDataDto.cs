using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.chat_commands.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);

    public readonly SafeField<char> CommandIdentifier = new SafeField<char>('!');

    public readonly SafeField<State> VerboseState = new SafeField<State>(State.Disabled);

    public readonly SafeField<string> BaseTitle = new SafeField<string>(string.Empty);

    public readonly SafeField<bool> SendWhisperIfPossible = new SafeField<bool>(false);
    
    public readonly SafeField<bool> AiOnMention = new SafeField<bool>(false);
    
    public readonly SafeField<int> MentionCmdId = new SafeField<int>(12);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        char commandIdentifier,
        State verboseState,
        string baseTitle,
        bool sendWhisperIfPossible,
        bool aiOnMention,
        int mentionCmdId) {
        ServiceState.Value = serviceState;
        CommandIdentifier.Value = commandIdentifier;
        VerboseState.Value = verboseState;
        BaseTitle.Value = baseTitle;
        SendWhisperIfPossible.Value = sendWhisperIfPossible;
        AiOnMention.Value = aiOnMention;
        MentionCmdId.Value = mentionCmdId;
    }
}