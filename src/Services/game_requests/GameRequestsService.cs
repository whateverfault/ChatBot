using ChatBot.Services.interfaces;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    public override string Name { get; }
    public override Options Options { get; }
}