using System.Globalization;
using ChatBot.twitchAPI.interfaces;
using ChatBot.utils;
using Microsoft.Extensions.Logging;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBot.twitchAPI;

public class Bot {
    private readonly string _savePath = Path.Combine(Shared.saveDirectory, "options.json");
    
    private ITwitchClient? _client;
    private Options _options = null!;
    private CommandsHandler? _cmdsHandler;
    private readonly ILogger<TwitchClient> _logger = null!;
    

    public void Start() 
    {
        if (_client == null) Init();

        IApiSettings apiSettings = new ApiSettings();
        apiSettings.AccessToken = _options.OAuth;
        apiSettings.ClientId = _options.ClientId;
        apiSettings.Secret = _options.Secret;
        
        _client!.OnChatCommandReceived += _cmdsHandler!.Handle;
        _client.OnMessageReceived += Client_OnMessageReceived;
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnConnected += Client_OnConnected;
        _client.OnLog += Client_OnLog;

        _client.Connect();
    }

    public void Stop()
    {
        _client?.Disconnect();
    }
    
    private void Init() {
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
        Console.WriteLine($"{e.DateTime.ToString(CultureInfo.InvariantCulture)}: {e.BotUsername} - {e.Data}");
    }
    
    private ConnectionCredentials GetCredentials() {
        string? channel;
        var clientId = "";
        var secret = "";
        var shouldCreateReward = false;
        ConsoleKeyInfo key;
        if (JsonUtils.TryRead(_savePath, out _options!)) {
            Console.WriteLine("Restore Saved Data?(Y/n)");
            key = Console.ReadKey();
            Console.Clear();
            if (key.Key != ConsoleKey.N) {
                return RestoreCredentials(out channel);
            }
        }

        Console.Write("Twitch username: ");
        var userName = Console.ReadLine();
        
        Console.Clear();
        Console.Write("OAuth token(do not show anyone): ");
        var token = Console.ReadLine();
        
        Console.Clear();
        Console.Write("Channel: ");
        channel = Console.ReadLine();
        
        Console.Clear();
        Console.WriteLine("Should I Create a Reward for Game Requests?(y/N)");
        key = Console.ReadKey();
        Console.Clear();
        if (key.Key == ConsoleKey.Y) {
            Console.Write("Then There Are a Few More Options...");
            Console.ReadKey();
            
            Console.Clear();
            Console.Write("ClientId: ");
            clientId = Console.ReadLine();
            
            Console.Clear();
            Console.Write("Secret: ");
            secret = Console.ReadLine();
            
            shouldCreateReward = true;
        }
        
        Console.Clear();
        Console.WriteLine("Should I Save This Data?(y/N)");
        key = Console.ReadKey();
        Console.Clear();
        _options = new Options(userName, token, channel, shouldCreateReward, clientId, secret);
        if (key.Key == ConsoleKey.Y) {
            JsonUtils.WriteSafe(_savePath, Shared.saveDirectory, _options);
            Console.WriteLine($"Data has been successfully saved to {_savePath}.");
        }
        
        return new ConnectionCredentials(userName, token);
    }

    private ConnectionCredentials RestoreCredentials(out string? channel) {
        channel = string.Empty;
        if (string.IsNullOrEmpty(_options.Username) || string.IsNullOrEmpty(_options.OAuth)|| string.IsNullOrEmpty(_options.Channel)) {
            Console.WriteLine($"Corrupted Save at {_savePath}\nTry to delete it or rename it.");
            return new ConnectionCredentials("", "");
        }
        channel = _options.Channel;
        return new ConnectionCredentials(_options.Username, _options.OAuth);
    }
}