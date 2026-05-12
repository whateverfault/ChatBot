namespace ChatBot.api.demon_list.data;

public class UserProfile {
    public string Id { get; set; }
    public int Rank { get; set; }
    public string Points { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public LevelInfo? Hardest { get; set; }
    
    
    public UserProfile(string id, int rank, string points, string displayName, string username, LevelInfo? hardest) {
        Id = id;
        Rank = rank;
        Points = points;
        DisplayName = displayName;
        Hardest = hardest;
        Username = username;
    }
}