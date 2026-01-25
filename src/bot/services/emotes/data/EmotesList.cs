namespace ChatBot.bot.services.emotes.data;

public static class EmotesList {
    public static readonly IReadOnlyDictionary<EmoteId, Emote> Emotes = new Dictionary<EmoteId, Emote> { 
                                                                                                             {
                                                                                                                 EmoteId.Sad, 
                                                                                                                 new Emote(EmoteId.Sad, "Sadge")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Wait, 
                                                                                                                 new Emote(EmoteId.Wait, "Waiting")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Rizz, 
                                                                                                                 new Emote(EmoteId.Rizz, "RIZZ")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Jail, 
                                                                                                                 new Emote(EmoteId.Jail, "sillyJAIL")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Laugh, 
                                                                                                                 new Emote(EmoteId.Laugh, "GAGAGA")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Aga, 
                                                                                                                 new Emote(EmoteId.Aga, "aga")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Like, 
                                                                                                                 new Emote(EmoteId.Like, "LIKE")
                                                                                                             },
                                                                                                             {
                                                                                                                 EmoteId.Happy, 
                                                                                                                 new Emote(EmoteId.Like, "happi")
                                                                                                             },
                                                                                                         };

    public static Emote GetEmote(EmoteId id) {
        return Emotes[id];
    }
}