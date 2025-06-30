namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class SearchOptions {
    public required GameSearchOptions Games { get; set; }
    public required UserSearchOptions Users { get; set; }
    public required ListSearchOptions Lists { get; set; }
    public required string Filter { get; set; }
    public int Sort { get; set; }
    public int Randomizer { get; set; }
}