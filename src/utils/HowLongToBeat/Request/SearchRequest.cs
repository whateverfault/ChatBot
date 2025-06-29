using ChatBot.utils.HowLongToBeat.Request.Data;

namespace ChatBot.utils.HowLongToBeat.Request;

public class SearchRequest {
    public string SearchType { get; set; }
    public List<string> SearchTerms { get; set; }
    public int SearchPage { get; set; }
    public int Size { get; set; }
    public SearchOptions SearchOptions { get; set; }
    public bool UseCache { get; set; }
}