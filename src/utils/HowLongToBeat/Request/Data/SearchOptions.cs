namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class SearchOptions {
    public GameSearchOptions Games { get; set; }
    public UserSearchOptions Users { get; set; }
    public ListSearchOptions Lists { get; set; }
    public string Filter { get; set; }
    public int Sort { get; set; }
    public int Randomizer { get; set; }
}