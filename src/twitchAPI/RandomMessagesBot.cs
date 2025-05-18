using System.Globalization;
using ChatBot.twitchAPI.interfaces;
using ChatBot.utils;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBot.twitchAPI;

public class RandomMessagesBot : Bot {
    private readonly string _savePath = Path.Combine(Shared.saveDirectory, "random_bot_options.json");
    private readonly string _logsPath = Path.Combine(Shared.saveDirectory, "logs.json");
    
    private ITwitchClient? _client;
    private Options _options = null!;
    private List<string>? _logs = new();
    private int _counter ;
    private readonly int _counterValueToGenerate = 15;
    private readonly ILogger<TwitchClient> _logger = null!;

    public override void Start() 
    {
        if (_client == null) Init();
        
        _client!.OnMessageReceived += Client_OnMessageReceived;
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnLog += Client_OnLog;

        _client.Connect();
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

        JsonUtils.TryRead(_logsPath, out _logs);
        _logs ??= [];
    }
    
    private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine($"Joined {e.Channel}");
    }
    
    private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        _logs!.Add(e.ChatMessage.Message);
        _counter++;
        var randomValue = Random.Shared.Next(0, _logs.Count);
        var randomness = _counterValueToGenerate-Random.Shared.Next(0, (int)(_counterValueToGenerate*0.5));
        if (_counter%randomness == 0) {
            _counter = 0;
            _client!.SendMessage(e.ChatMessage.Channel, _logs[randomValue]);
            JsonUtils.WriteSafe(_logsPath, Shared.saveDirectory, _logs);
            Console.WriteLine($"Randomness: {randomness}");
        }
        Console.WriteLine($"Message received: {e.ChatMessage.Message}");
        Console.WriteLine($"Counter Updated: {_counter}");
    }
    
    private void Client_OnLog(object? sender, OnLogArgs e)
    {
        Console.WriteLine($"{e.DateTime.ToString(CultureInfo.InvariantCulture)}: {e.BotUsername} - {e.Data}");
    }
    
    private ConnectionCredentials GetCredentials() {
        string? channel;
        var clientId = "";
        var broadcasterId = "";
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
        Console.WriteLine("Should I Use a Reward for Game Requests?(y/N)");
        key = Console.ReadKey();
        Console.Clear();
        if (key.Key == ConsoleKey.Y) {
            Console.Write("Then There Are a Few More Options...");
            Console.ReadKey();
            
            Console.Clear();
            Console.Write("ClientId: ");
            clientId = Console.ReadLine();
            
            Console.Clear();
            Console.Write("BroadcasterId: ");
            broadcasterId = Console.ReadLine();
            
            Console.Clear();
            Console.Write("Secret: ");
            secret = Console.ReadLine();
            
            shouldCreateReward = true;
        }
        
        Console.Clear();
        Console.WriteLine("Should I Save This Data?(Y/n)");
        key = Console.ReadKey();
        Console.Clear();
        _options = new Options(userName, token, channel, shouldCreateReward, clientId, broadcasterId, secret);
        if (key.Key == ConsoleKey.N) {
            return new ConnectionCredentials(userName, token);
        }
        
        JsonUtils.WriteSafe(_savePath, Shared.saveDirectory, _options);
        Console.WriteLine($"Data has been successfully saved to {_savePath}.");
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