using System.Globalization;
using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using ChatBot.twitchAPI.interfaces;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBot.twitchAPI;

public class TwitchChatBot : Bot {
    public override Options Options => _options;

    private ITwitchClient? _client;
    private ChatBotOptions _options = null!;
    private CommandsHandler? _cmdsHandler;
    private readonly ILogger<TwitchClient> _logger = null!;


    public override void Start() 
    {
        if (_client == null) Init();
        
        _client!.OnChatCommandReceived += _cmdsHandler!.Handle;
        _client.OnMessageReceived += Client_OnMessageReceived;
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnConnected += Client_OnConnected;
        _client.OnLog += Client_OnLog;

        _client.Connect();
    }

    public override void Login() {
        throw new NotImplementedException();
    }

    public override void Enable() {
        throw new NotImplementedException();
    }

    public override void Disable() {
        throw new NotImplementedException();
    }

    public override ErrorCode GetClient(out ITwitchClient client) {
        throw new NotImplementedException();
    }

    public override State GetState() {
        throw new NotImplementedException();
    }

    public override void Toggle() {
        throw new NotImplementedException();
    }

    private void Init() {
        _options = new ChatBotOptions();
        
        var credentials = GetCredentials();
        var clientOptions = new ClientOptions
                            {
                                MessagesAllowedInPeriod = 750,
                                ThrottlingPeriod = TimeSpan.FromSeconds(30)
                            };
        var customClient = new WebSocketClient(clientOptions);
        
        
        _client = new TwitchClient(customClient, logger: _logger);
        _client.Initialize(credentials, _options.Channel);

        _cmdsHandler = new ChatCommandsHandler(_client);
    }
    
    private void Client_OnConnected(object? sender, OnConnectedArgs e) {
        Console.WriteLine($"Connected to {e.AutoJoinChannel}");
    }
    
    private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine($"Joined {e.Channel}");
    }
    
    private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        Console.WriteLine($"Message received: {e.ChatMessage.Message}");
    }
    
    private void Client_OnLog(object? sender, OnLogArgs e)
    {
        if (_options.ShouldPrintTwitchLogs) {
            Console.WriteLine($"{e.DateTime.ToString(CultureInfo.InvariantCulture)}: {e.BotUsername} - {e.Data}");
        }
    }
    
    private ConnectionCredentials GetCredentials() {
        ConsoleKeyInfo key;
        
        if (_options.Load()) {
            Console.WriteLine("Restore Saved Data?(Y/n)");
            key = Console.ReadKey();
            Console.Clear();
            if (key.Key != ConsoleKey.N) {
                return new ConnectionCredentials(_options.Username, _options.OAuth);
            }
        }
        
        _options.ToDefaults();
        Console.Write("Twitch username: ");
        _options.SetUsername(Console.ReadLine()!);
        
        Console.Clear();
        Console.Write("OAuth token(do not show anyone): ");
        _options.SetOAuth(Console.ReadLine()!);
        
        Console.Clear();
        Console.Write("Channel: ");
        _options.SetChannel(Console.ReadLine()!);
        
        Console.Clear();
        Console.WriteLine("Should I Save This Data?(Y/n)");
        key = Console.ReadKey();
        Console.Clear();
        
        if (key.Key == ConsoleKey.N) {
            return new ConnectionCredentials(_options.Username, _options.OAuth);
        }
        
        _options.Save();
        return new ConnectionCredentials(_options.Username, _options.OAuth);
    }
}