namespace ChatBot.bot.services.localization.data;

public enum Lang {
    Ru,
    Eng,
}

public delegate object DynamicArgHandler();

public class Localization {
    private readonly Dictionary<Lang, string> _variants;

    public IReadOnlyDictionary<Lang, string> Variants => _variants;
    public readonly DynamicArgHandler[] Args; 
    

    public Localization(string ru, string eng, params DynamicArgHandler[] args) {
        Args = args;
        
        _variants = new Dictionary<Lang, string> {
                                                     {
                                                         Lang.Ru,
                                                         ru
                                                     },
                                                     {
                                                         Lang.Eng,
                                                         eng
                                                     },
                                                 };
    }
}