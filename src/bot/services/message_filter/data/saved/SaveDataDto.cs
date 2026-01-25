using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using TwitchAPI.api.data.responses.GetUserInfo;

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
    
    public readonly SafeField<List<UserInfo>> BannedUsers = 
        new SafeField<List<UserInfo>>([
                                          new UserInfo(
                                                       "19264788",
                                                       "nightbot",
                                                       "Nightbot",
                                                       string.Empty,
                                                       string.Empty,
                                                       string.Empty
                                                       ),
                                          new UserInfo(
                                                       "541450924",
                                                       "creatisbot",
                                                       "CreatisBot",
                                                       string.Empty,
                                                       string.Empty,
                                                       string.Empty
                                                      ),
                                          new UserInfo(
                                                       "1564983",
                                                       "moobot",
                                                       "Moobot",
                                                       string.Empty,
                                                       string.Empty,
                                                       string.Empty
                                                      ),
                                          new UserInfo(
                                                       "19264788",
                                                       "nightbot",
                                                       "Nightbot",
                                                       string.Empty,
                                                       string.Empty,
                                                       string.Empty
                                                      ),
                                          new UserInfo(
                                                       "100135110",
                                                       "streamelements",
                                                       "StreamElements",
                                                       string.Empty,
                                                       string.Empty,
                                                       string.Empty
                                                      ),
                                      ]);
    

    public SaveDataDto() { }
    
    public SaveDataDto(
        List<Filter> filters,
        List<UserInfo> bannedUsers,
        State state) {
        Filters.Value = filters;
        BannedUsers.Value = bannedUsers;
        State.Value = state;
    }
}