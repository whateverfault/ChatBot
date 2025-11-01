using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_ads.Data;

namespace ChatBot.cli.CliNodes.Directories.ChatAds;

public class CliNodeDynamicChatAdsDirectory : CliNodeDirectory {
    private readonly AddChatAdHandler _addHandler;
    private readonly RemoveHandler _removeHandler;
    private readonly CliNodeStaticDirectory _content;
    private readonly CliState _state;
    
    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }
    
    
    public CliNodeDynamicChatAdsDirectory(
        string text,
        string addText,
        string removeText,
        CliState state) {
        Text = text;
        _state = state;
        _addHandler = _state.Data.ChatAds.AddChatAd;
        _removeHandler = _state.Data.ChatAds.RemoveChatAd;

        _content = new CliNodeStaticDirectory(
                                              "Content",
                                              state,
                                              true,
                                              []
                                              );

        _content.AddNode(
                         new CliNodeText(
                                         "-----------------------------------",
                                         false,
                                         true,
                                         1
                                        )
                         );

        var chatAds = _state.Data.ChatAds.GetChatAds();
        foreach (var cmd in chatAds) {
            _content.AddNode(ChatAdToNode(cmd));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeChatAdAdd(addText, Add),
                        new CliNodeRemove(removeText, Remove),
                        _content,
                    ];
        }

        _state.Data.ChatAds.OnChatAdAdded += (_, chatAd) => {
                                                 _content.AddNode(ChatAdToNode(chatAd));
                                             };
        
        _state.Data.ChatAds.OnChatAdRemoved += (_, index) => { 
                                                   _content.RemoveNode(index+2);
                                             };
    }
    
    private void Add(ChatAd chatAd) {
        _addHandler.Invoke(chatAd);
    }
    
    private bool Remove(int index) {
        if (index < 0 || index >= _content.Nodes.Count-2) {
            return false;
        }
        
        return _removeHandler.Invoke(index);
    }
    
    private CliNodeStaticDirectory ChatAdToNode(ChatAd chatAd) {
        var node = new CliNodeStaticDirectory(
                                              chatAd.GetName(),
                                              _state,
                                              true,
                                              [
                                              new CliNodeString(
                                                                "Name",
                                                                chatAd.GetName,
                                                                CliNodePermission.Default,
                                                                chatAd.SetName
                                                                ),
                                              new CliNodeString(
                                                                "Output",
                                                                chatAd.GetOutput,
                                                                CliNodePermission.Default,
                                                                chatAd.SetOutput
                                                               ),
                                              new CliNodeTime(
                                                                "Cooldown",
                                                                chatAd.GetCooldown,
                                                                CliNodePermission.Default,
                                                                chatAd.SetCooldown,
                                                                isUnixEpoch: false
                                                               ),
                                              new CliNodeLong(
                                                              "Message Threshold",
                                                              chatAd.GetThreshold,
                                                              CliNodePermission.Default,
                                                              chatAd.SetThreshold
                                                             ),
                                              new CliNodeEnum(
                                                                "State",
                                                                chatAd.GetStateAsInt,
                                                                typeof(State),
                                                                CliNodePermission.Default,
                                                                chatAd.StateNext
                                                               ),
                                              ],
                                              chatAd.GetName
                                              );
        return node;
    }
}