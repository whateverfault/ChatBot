using ChatBot.utils.HowLongToBeat.Request.Data;

namespace ChatBot.utils.HowLongToBeat.Request;

public class SearchRequest {
    public required string SearchType { get; set; }
    public required List<string> SearchTerms { get; set; }
    public int SearchPage { get; set; }
    public int Size { get; set; }
    public required SearchOptions SearchOptions { get; set; }
    public bool UseCache { get; set; }
}