using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs.data;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.bot.services.message_randomizer.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<Message> LastGeneratedMessage = new SafeField<Message>(Message.Empty);

    public readonly SafeField<MessageState> MessageState = new SafeField<MessageState>(message_randomizer.MessageState.NotGuessed);
    
    public readonly SafeField<int> CounterMax = new SafeField<int>(25);

    public readonly SafeField<State> Randomness = new SafeField<State>(State.Disabled);

    public readonly SafeField<Range> Spreading = new SafeField<Range>(new Range(15, 30));


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        Message lastGeneratedMessage, 
        MessageState messageState,
        int counterMax,
        State randomness,
        Range spreading) {
        ServiceState.Value = serviceState;
        MessageState.Value = messageState;
        LastGeneratedMessage.Value = lastGeneratedMessage;
        CounterMax.Value = counterMax;
        Randomness.Value = randomness;
        Spreading.Value = spreading;
    }
}