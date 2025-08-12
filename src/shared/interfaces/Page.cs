namespace ChatBot.shared.interfaces;

public static class Page {
    public const string PageTerminator = "--";


    public static int[] CalculatePages(List<string> blocks) {
        var pages = new List<int>();
        var charCounter = 0;
        var pageTerminatorsCount = 0;

        foreach (var t in blocks) {
            if (t == PageTerminator) {
                charCounter = 0;
                pageTerminatorsCount++;
                continue;
            }
            
            charCounter += t.Length;
            pages.Add(charCounter/(Constants.MaxMessageCapacity-20)+1+pageTerminatorsCount);
        }

        return pages.ToArray();
    }
}