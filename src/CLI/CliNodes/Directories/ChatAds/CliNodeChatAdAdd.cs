using ChatBot.services.chat_ads.Data;

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
        var name = Console.ReadLine() ?? "--";
        
        Console.Write("Ad Output: ");
        var output = Console.ReadLine() ?? "--";
        
        Console.Write("Ad Cooldown: ");
        if (!long.TryParse(Console.ReadLine(), out var cooldown)) {
            cooldown = -1;
        }
        
        var chatCmd = new ChatAd(name, output, cooldown);
        _add.Invoke(chatCmd);
    }
}