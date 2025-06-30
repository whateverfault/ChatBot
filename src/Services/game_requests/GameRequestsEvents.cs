using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;

namespace ChatBot.Services.game_requests;

public class GameRequestsEvents : ServiceEvents {
    private GameRequestsService _service = null!;
    private Bot _bot = null!;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (GameRequestsService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        if (subscribed) return;
        base.Subscribe();
        _bot.OnMessageReceived += _service.HandleMessage;
    }
}