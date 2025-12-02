using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;

namespace ChatBot.bot.services.stream_state_checker.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);

    public readonly SafeField<StreamState> StreamState = new SafeField<StreamState>(
                                                                                    new StreamState()
                                                                                   );

    public readonly SafeField<StreamStateMeta> StreamStateMeta = new SafeField<StreamStateMeta>(
                                                                                                new StreamStateMeta(120, 0)
                                                                                               );


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        StreamState streamState,
        StreamStateMeta streamStateMeta) {
        ServiceState.Value = serviceState;
        StreamState.Value = streamState;
        StreamStateMeta.Value = streamStateMeta;
    }
}