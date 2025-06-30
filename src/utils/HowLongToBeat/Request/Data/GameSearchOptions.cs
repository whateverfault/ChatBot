namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class GameSearchOptions {
    public int UserId { get; set; }
    public required string Platform { get; set; }
    public required string SortCategory { get; set; }
    public required string RangeCategory { get; set; }
    public required RangeTime RangeTime { get; set; }
    public required Gameplay Gameplay { get; set; }
    public required RangeYear RangeYear { get; set; }
    public required string Modifier { get; set; }
}