using ChatBot.Shared;

namespace ChatBot.twitchAPI.interfaces;

public static class Page {
    public static int[] CalculatePages(string[] blocks) {
        var pages = new List<int>();
        var charCounter = 0;
        
        foreach (var t in blocks) {
            charCounter += t.Length;
            pages.Add((charCounter/(Constants.MaxMessageCapacity-20))+1);
        }
        
        return pages.ToArray();
    }
}