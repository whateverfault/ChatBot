using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.message_filter.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> State = new SafeField<State>(bot.interfaces.State.Enabled);

    public readonly SafeField<List<Filter>> Filters = new SafeField<List<Filter>>(
                                                                                  [
                                                                                      new Filter(
                                                                                                 "Level Requests",
                                                                                                 @"\b\d{8,11}\b",
                                                                                                 true
                                                                                                ),
                                                                                      new Filter(
                                                                                                 "Special Symbols",
                                                                                                 "^[!@~]+",
                                                                                                 true
                                                                                                ),
                                                                                  ]
                                                                                 );


    public SaveDataDto() { }
    
    public SaveDataDto(
        List<Filter> filters,
        State state) {
        Filters.Value = filters;
        State.Value = state;
    }
}