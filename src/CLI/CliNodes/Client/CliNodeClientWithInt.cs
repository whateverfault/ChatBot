using ChatBot.shared.Handlers;
using TwitchLib.Client.Interfaces;

namespace ChatBot.CLI.CliNodes.Client;

public class CliNodeClientWithInt : CliNode {
    private readonly ClientHandler _clientHandler;
    private readonly IntGetter _getter;
    private ITwitchClient _client;
    private string _channel;
    
    protected override string Text { get; }

    
    public CliNodeClientWithInt(string text, ClientHandler clientHandler, IntGetter getter) {
        Text = text;
        _clientHandler = clientHandler;
        _getter = getter;
    }

    public override int PrintValue(int index) {
        base.PrintValue(index);
        Console.Write($" - {_getter.Invoke()}");
        return 0;
    }

    public override void Activate(CliState state) {
        var err = state.Data.Bot.TryGetClient(out _client);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        
        _channel = state.Data.Bot.Options.GetChannel();
        _clientHandler.Invoke(_client, _channel);
    }
}