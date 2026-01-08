using System.Net;
using System.Text;
using System.Web;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.scopes;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.api;
using TwitchAPI.client;
using TwitchAPI.client.credentials;
using TwitchAPI.client.data;

namespace ChatBot.bot.chat_bot;

public delegate void NoArgs();

public sealed class TwitchChatBot : Bot {
    private static TwitchChatBot? _instance;
    public static TwitchChatBot Instance => _instance ??= new TwitchChatBot();

    private readonly object _startLock = new object();
    private bool _starting;
    private bool _initialized;

    public override TwitchApi Api { get; } = new TwitchApi(Network.HttpClient);

    public override ChatBotOptions Options { get; }

    public override event EventHandler<ChatMessage>? OnMessageReceived;
    public event NoArgs? OnInitialized;

    private TwitchChatBot() {
        Options = new ChatBotOptions();

        var chatCommands =
            (ChatCommandsService)Services.Get(ServiceId.ChatCommands);

        var twitchClientConfig = new TwitchClientConfig(
                                                        new AutoReconnectConfig(),
                                                        Api,
                                                        chatCommands.Options.CommandIdentifier
                                                       );

        ReplaceClient(new TwitchClient(twitchClientConfig));
    }

    private void ReplaceClient(TwitchClient newClient) {
        if (Options.Client != null) {
            UnsubscribeFromEvents();
        }

        Options.UpdateClient(newClient);
        SubscribeToEvents();
    }

    public override async Task Start() {
        lock (_startLock) {
            if (_starting) {
                return;
            }

            _starting = true;
        }

        try {
            ErrorHandler.LogMessage(
                LogLevel.Info,
                $"Connecting to {Options.Credentials.Channel}..."
            );

            Services.Init();

            await InitConnectionAsync();
        }
        finally {
            lock (_startLock) {
                _starting = false;
            }
        }
    }

    public override async Task Stop() {
        await StopInternal();
    }

    public override ErrorCode TryGetClient(out ITwitchClient? client) {
        client = GetClient(); 
        
        return _initialized ? ErrorCode.None : ErrorCode.NotInitialized;
    }

    public override ITwitchClient? GetClient() {
        return Options.Client;
    }

    private async Task StopInternal() {
        if (Options.Client == null || !_initialized) {
            return;
        }

        Services.Kill();

        UnsubscribeFromEvents();
        await Options.Client.Disconnect();

        _initialized = false;

        ErrorHandler.LogMessage(LogLevel.Info, "Disconnected.");
    }

    private async Task InitConnectionAsync() {
        if (!ValidateSave()) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.CorruptedCredentials);
            return;
        }

        await StopInternal();
        SubscribeToEvents();

        if (Options.Client == null) {
            return;
        }

        await Options.Client.Initialize(Options.Credentials);

        if (Options.Client.Credentials == null) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ConnectionFailed);
            return;
        }

        _initialized = true;
    }

    private void SubscribeToEvents() {
        if (Options.Client == null) {
            return;
        }

        UnsubscribeFromEvents();
        
        Options.Client.OnMessageReceived += HandleMessageReceived;
        Options.Client.OnError += HandleError;
        Options.Client.OnConnected += HandleConnected;
    }

    private void UnsubscribeFromEvents() {
        if (Options.Client == null) {
            return;
        }

        Options.Client.OnMessageReceived -= HandleMessageReceived;
        Options.Client.OnError -= HandleError;
        Options.Client.OnConnected -= HandleConnected;
    }

    private void HandleMessageReceived(object? sender, ChatMessage message) {
        var handler = OnMessageReceived;
        if (handler != null) {
            handler(sender, message);
        }
    }

    private void HandleConnected(object? sender, EventArgs e) {
        ErrorHandler.LogMessage(
            LogLevel.Info,
            $"Connected to {Options.Credentials.Channel}"
        );

        var handler = OnInitialized;
        if (handler != null) {
            handler();
        }
    }

    private void HandleError(object? sender, string message) {
        ErrorHandler.LogMessage(LogLevel.Error, message);
    }

    public string GetChannel() {
        if (Options.Client?.Credentials?.Channel != null) {
            return Options.Client.Credentials.Channel;
        }

        return string.IsNullOrEmpty(Options.Credentials.Channel)
            ? "Empty"
            : Options.Credentials.Channel;
    }

    public async Task SetChannel(string username) {
        if (string.IsNullOrEmpty(username)) {
            return;
        }
        
        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    username,
                    Options.Credentials.Oauth,
                    Options.Credentials.ChannelOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateChannel(username);
        Options.UpdateCredentials();
    }

    public async Task SetOauth(string oauth) {
        if (string.IsNullOrEmpty(oauth)) {
            return;
        }
        
        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    Options.Credentials.Channel,
                    oauth,
                    Options.Credentials.ChannelOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateOauth(oauth);
        Options.UpdateCredentials();
    }

    public async Task SetChannelOauth(string channelOauth) {
        if (string.IsNullOrEmpty(channelOauth)) {
            return;
        }

        if (Options.Client?.Credentials == null) {
            Options.SetCredentials(
                new ConnectionCredentials(
                    Options.Credentials.Channel,
                    Options.Credentials.Oauth,
                    channelOauth
                )
            );
            
            return;
        }

        await Options.Client.UpdateChannelOauth(channelOauth);
        Options.UpdateCredentials();
    }

    public int CurAuthLevelAsInt() {
        return (int)Options.CurAuthLevel;
    }
    
    public bool HasBroadcasterAuth() {
        return Options.HasBroadcasterAuth;
    }
    
    public async Task GetAuthorization() {
        var listener = new HttpListener();
        listener.Prefixes.Add($"{Constants.RedirectUri}/");
        listener.Start();

        OpenBrowser();

        while (true) {
            var context = await listener.GetContextAsync();
            var path = context.Request.Url?.AbsolutePath;

            switch (path) {
                case "/": {
                    await SendLandingPage(context);
                    break;
                }
                case "/token": {
                    var scopesService = (ScopesService)Services.Get(ServiceId.Scopes);
                    var query = context.Request.Url?.Query;

                    var oauth = string.Empty;
                    var scopes = string.Empty;
                    
                    if (query != null) {
                        var parsed = HttpUtility.ParseQueryString(query);
                        
                        oauth = parsed["access_token"];
                        scopes = parsed["scope"];
                    }

                    if (!string.IsNullOrEmpty(oauth) 
                     && !string.IsNullOrEmpty(scopes)) {
                        var scopesPreset = scopesService.GetScopesPreset(scopes);

                        if (scopesPreset is ScopesPreset.Chatter or ScopesPreset.Moderator) {
                            await SetOauth(oauth);
                            
                            Options.SetCurAuthLevel(scopesPreset);
                        }else {
                            await SetChannelOauth(oauth);
                            
                            Options.SetHasBroadcasterAuth(true);
                        }
                    }
                    
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    listener.Stop();
                    return;
                }
                default:
                    context.Response.StatusCode = 204;
                    context.Response.Close();
                    break;
            }
        }
    }

    private void OpenBrowser() {
        var scopes = (ScopesService)Services.Get(ServiceId.Scopes);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                         {
                                             FileName = Instance.Api.BuildAuthorizationUrl(
                                                  Constants.ClientId,
                                                  Constants.RedirectUri,
                                                  scopes.GetScopesString()
                                                 ),
                                             UseShellExecute = true,
                                         });
    }

    private async Task SendLandingPage(HttpListenerContext context) {
        const string page = """
                            <!DOCTYPE html>
                            <html>
                            <body>
                            You may close this page now.
                            <script>
                              if (window.location.hash.length > 1) {
                                fetch('/token?' + window.location.hash.substring(1));
                              }
                            </script>
                            </body>
                            </html>
                            """;

        var buffer = Encoding.UTF8.GetBytes(page);
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.OutputStream.Close();
    }
    
    private bool ValidateSave() {
        return !string.IsNullOrEmpty(Options.Credentials.Channel)
            && !string.IsNullOrEmpty(Options.Credentials.Oauth);
    }
}
