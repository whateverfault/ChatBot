using ChatBot.bot;
using ChatBot.services.interfaces;

namespace ChatBot.services.game_requests;

public class GameRequestsEvents : ServiceEvents {
    private GameRequestsService _gameRequests = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _gameRequests = (GameRequestsService)service;
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        TwitchChatBot.Instance.OnMessageReceived += _gameRequests.HandleMessage;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnMessageReceived -= _gameRequests.HandleMessage;
    }
}