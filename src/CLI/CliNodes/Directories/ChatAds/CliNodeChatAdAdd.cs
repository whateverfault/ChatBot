using ChatBot.bot.services.chat_ads.data;
using ChatBot.bot.shared.handlers;

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
        var name = IoHandler.ReadLine("Ad Name: ");
        if (string.IsNullOrEmpty(name)) return;
        
        var output = IoHandler.ReadLine("Ad Output: ");
        if (string.IsNullOrEmpty(output)) return;
        
        var cooldownStr = IoHandler.ReadLine("Ad Cooldown: ");
        if (!long.TryParse(cooldownStr, out var cooldown)) {
            return;
        }
        
        var chatCmd = new ChatAd(name, output, cooldown);
        _add.Invoke(chatCmd);
    }
}