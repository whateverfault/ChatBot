using ChatBot.api.demon_list.aredl.data;
using ChatBot.api.demon_list.glist.data;

namespace ChatBot.api.demon_list.data;

public class LevelInfo {
    public string LevelId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; }
    public string Points { get; set; }
    public string VerificationLink { get; set; }
    public string? Creator { get; set; }
    public float? EdelEnjoyment { get; set; }
    public string? NlwTier { get; set; }
    public bool Platformer { get; set; }
    
    
    public LevelInfo(
        string levelId,
        int position,
        string name,
        string points,
        string verificationLink,
        string? creator = null,
        float? edelEnjoyment = null,
        string? nlwTier = null,
        bool platformer = false) {
        LevelId = levelId;
        Position = position;
        Name = name;
        Creator = creator;
        Points = points;
        VerificationLink = verificationLink;
        EdelEnjoyment = edelEnjoyment;
        NlwTier = nlwTier;
        Platformer = platformer;
    }

    public LevelInfo(AredlLevelInfo levelInfo) {
        LevelId = levelInfo.Id;
        Position = levelInfo.Position;
        Name = levelInfo.Name;
        Creator = string.Empty;
        Points = levelInfo.Points.ToString();
        VerificationLink = string.Empty;
        EdelEnjoyment = levelInfo.EdelEnjoyment;
        NlwTier = levelInfo.NlwTier;
        Platformer = levelInfo.Platformer;
    }
    
    public LevelInfo(GListLevelInfo levelInfo) {
        LevelId = levelInfo.LevelId.ToString();
        Position = levelInfo.Position;
        Name = levelInfo.Name;
        Creator = levelInfo.Creator;
        Points = levelInfo.Points;
        VerificationLink = levelInfo.VerificationLink;
        EdelEnjoyment = null;
        NlwTier = null;
        Platformer = false;
    }
    
    public LevelInfo(ShortLevelInfo levelInfo) {
        Position = levelInfo.Position;
        Name = levelInfo.Name;
        VerificationLink = levelInfo.VerificationLink;
        EdelEnjoyment = null;
        NlwTier = null;
        Platformer = false;
        Points = string.Empty;
        Creator = string.Empty;
        LevelId = string.Empty;
    }
}