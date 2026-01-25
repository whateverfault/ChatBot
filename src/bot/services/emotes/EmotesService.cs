using ChatBot.bot.services.emotes.data;
using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.emotes;

public class EmotesService : Service {
    public override EmotesOptions Options { get; } = new EmotesOptions();
    
    
    public string Get7TvEmote(EmoteId id) {
        if (!Options.Use7Tv) {
            return string.Empty;
        }

        var emote = EmotesList.GetEmote(id);
        return emote.Text;
    }
}