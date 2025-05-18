namespace ChatBot.twitchAPI.interfaces;

public class Page {
    public static int[] CalculatePages(string[] blocks) {
        var pages = new List<int>();
        var char_counter = 0;
        
        foreach (var t in blocks) {
            char_counter += t.Length;
            pages.Add((char_counter/(Shared.MaxMessageCapacity-20))+1);
        }
        
        return pages.ToArray();
    }
}