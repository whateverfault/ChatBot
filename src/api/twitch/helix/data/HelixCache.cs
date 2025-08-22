using ChatBot.api.basic;
using ChatBot.api.twitch.helix.data.responses.GetUserInfo;

namespace ChatBot.api.twitch.helix.data;

public class HelixCache {
    public FixedSizeDictionary<string, UserInfo> UserInfoTable { get; private set; }


    public HelixCache(int maxSize) {
        UserInfoTable = new FixedSizeDictionary<string, UserInfo>(maxSize);
    }
}