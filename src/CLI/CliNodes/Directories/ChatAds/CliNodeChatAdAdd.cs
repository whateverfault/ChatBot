using ChatBot.bot.services.chat_ads.Data;

namespace ChatBot.cli.CliNodes.Directories.ChatAds;

public delegate void AddChatAdHandler(ChatAd chatCmd);

public class CliNodeChatAdAdd : CliNode {
    private readonly AddChatAdHandler _add;
    
    protected override string Text { get; }


    public CliNodeChatAdAdd(string text, AddChatAdHandler add) {
        Text = text;
        _add = add;
    }
    
    public override void Activate(CliState state) {
        Console.Write("Ad Name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrEmpty(name)) return;
        
        Console.Write("Ad Output: ");
        var output = Console.ReadLine();
        if (string.IsNullOrEmpty(output)) return;
        
        Console.Write("Ad Cooldown: ");
        if (!long.TryParse(Console.ReadLine(), out var cooldown)) {
            return;
        }
        
        var chatCmd = new ChatAd(name, output, cooldown);
        _add.Invoke(chatCmd);
    }
}