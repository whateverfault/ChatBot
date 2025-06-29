namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class GameSearchOptions {
    public int UserId { get; set; }
    public string Platform { get; set; }
    public string SortCategory { get; set; }
    public string RangeCategory { get; set; }
    public RangeTime RangeTime { get; set; }
    public Gameplay Gameplay { get; set; }
    public RangeYear RangeYear { get; set; }
    public string Modifier { get; set; }
}