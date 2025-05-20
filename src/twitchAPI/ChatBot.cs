using System.Runtime.InteropServices.JavaScript;
using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using ChatBot.twitchAPI.interfaces;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBot.twitchAPI;

public class ChatBot : Bot {
    private bool _loggedIn;
    private bool _initialized;
    private readonly ILogger<TwitchClient> _logger;
    private readonly ChatBotOptions _options = new();
    private ITwitchClient _client;
    

    public override Options Options => _options;


    private void Init() {
        try {
            var credentials = new ConnectionCredentials(_options.Username, _options.OAuth);
            var clientOptions = new ClientOptions {
                                                      MessagesAllowedInPeriod = 750,
                                                      ThrottlingPeriod = TimeSpan.FromSeconds(30)
                                                  };
            var customClient = new WebSocketClient(clientOptions);

            _client = new TwitchClient(customClient, logger: _logger);
            _client.Initialize(credentials, _options.Channel);
            ErrorHandler.LogErrorAndWait(ErrorCode.None);
            _initialized = true;
        } catch (Exception _) {
            ErrorHandler.LogErrorAndWait(ErrorCode.InvalidData);
        }
    }
    
    public override void Start() {
        if (!_loggedIn) {
            ErrorHandler.LogErrorAndWait(ErrorCode.NotLoggedIn);
            return;
        } 
        Init();
    }
    
    public override void Login() {
        _loggedIn = true;
        
        ConsoleKeyInfo key;
        
        if (_options.Load()) {
            Console.WriteLine("Restore Saved Data?(Y/n)");
            key = Console.ReadKey();
            Console.Clear();
            if (key.Key != ConsoleKey.N) {
                ErrorHandler.LogErrorAndWait(VerifySave());
                return;
            }
        }
        
        _options.ToDefaults();
        Console.Write("Twitch username: ");
        _options.SetUsername(Console.ReadLine() ?? "");
        
        Console.Clear();
        Console.Write("OAuth token(do not show anyone): ");
        _options.SetOAuth(Console.ReadLine() ?? "");
        
        Console.Clear();
        Console.Write("Channel: ");
        _options.SetChannel(Console.ReadLine() ?? "");
        
        Console.Clear();
        Console.WriteLine("Should I Save This Data?(Y/n)");
        key = Console.ReadKey();
        Console.Clear();
        
        if (key.Key == ConsoleKey.N) {
            return;
        }

        _options.Save();
        ErrorHandler.LogErrorAndWait(VerifySave());
    }

    public override void Enable() {
        _options.SetState(State.Enabled);
    }

    public override void Disable() {
        _options.SetState(State.Disabled);
    }

    public override ErrorCode GetClient(out ITwitchClient client) {
        client = _client;
        return !_initialized ? ErrorCode.NotInitialized : ErrorCode.None;
    }

    public override void Toggle() {
       _options.SetState(Options.ServiceState == State.Enabled? State.Disabled : State.Enabled);
    }
    
    private ErrorCode VerifySave() {
        if (_options.OAuth == null) return ErrorCode.LogInIssue;

        return ErrorCode.None;
    }
}