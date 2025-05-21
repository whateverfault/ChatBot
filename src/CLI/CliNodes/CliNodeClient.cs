using TwitchLib.Client.Interfaces;

namespace ChatBot.CLI.CliNodes;

public delegate void ClientHandler(ITwitchClient client, string channel);

public class CliNodeClient : CliNode {
    public override string Text { get; }
    public override CliNodeType Type { get; }
    public ClientHandler Action { get; }


    public CliNodeClient(string text, ClientHandler action) {
        Type = CliNodeType.Client;

        Text = text;
        Action = action;
    }
}