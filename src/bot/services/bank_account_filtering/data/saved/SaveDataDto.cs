using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.bank_account_filtering.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);
    
    public readonly SafeField<long> MinStreamLength = new SafeField<long>(3600);
    
    public readonly SafeField<long> LastActiveThreshold = new SafeField<long>(0);
    
    public readonly SafeField<long> LastOnline = new SafeField<long>(0);
    
    public readonly SafeField<long> StreamsTimeGapThreshold = new SafeField<long>(3600);
    
    public readonly SafeField<long> StreamsPassed = new SafeField<long>(0);
    
    public readonly SafeField<long> PassedStreamsThreshold = new SafeField<long>(5);
    
    public readonly SafeField<bool> StreamFiltered = new SafeField<bool>(false);
    
    
    public SaveDataDto(){}

    public SaveDataDto(
        State state,
        long minStreamLen,
        long lastActiveThreshold,
        long lastOnline,
        long streamsTimeGapThreshold,
        long streamsPassed,
        long passedStreamsThreshold,
        bool streamFiltered) {
        ServiceState.Value = state;
        MinStreamLength.Value = minStreamLen;
        LastActiveThreshold.Value = lastActiveThreshold;
        LastOnline.Value = lastOnline;
        StreamsTimeGapThreshold.Value = streamsTimeGapThreshold;
        StreamsPassed.Value = streamsPassed;
        PassedStreamsThreshold.Value = passedStreamsThreshold;
        StreamFiltered.Value = streamFiltered;
    }
}