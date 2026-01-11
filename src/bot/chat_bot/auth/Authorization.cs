using System.Net;
using System.Text;
using System.Web;
using ChatBot.bot.services.scopes;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using TwitchAPI.client.credentials;

namespace ChatBot.bot.chat_bot;

public enum AuthLevel{
    None = 0,
    Partitial,
    Full,
}

public static class Authorization {
    public static AuthLevel GetAuthorizationLevel(ConnectionCredentials creds, FullCredentials authedCreds) {
        var hasOauth = !string.IsNullOrEmpty(creds.Oauth);
        var hasBroadOauth = !string.IsNullOrEmpty(creds.BroadcasterOauth)
                         && creds.Channel.Equals(authedCreds.Broadcaster.Login, StringComparison.OrdinalIgnoreCase);

        if (hasOauth && hasBroadOauth) {
            return AuthLevel.Full;
        }
        
        return hasOauth ? 
                   AuthLevel.Partitial :
                   AuthLevel.None;
    }
    
    public static async Task<AuthorizationResponse> GetAuthorization() {
        var scopes = (ScopesService)Services.Get(ServiceId.Scopes);
        
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
                    var query = context.Request.Url?.Query;

                    var oauth = string.Empty;
                    
                    if (query != null) {
                        var parsed = HttpUtility.ParseQueryString(query);
                        
                        oauth = parsed["access_token"];
                    }
                    
                    var auth = scopes.GetScopesPreset() is ScopesPreset.Bot? 
                                   new AuthorizationResponse(bot: oauth) : 
                                   new AuthorizationResponse(broadcaster: oauth);
                    
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    listener.Stop();
                    
                    return auth;
                }
                default: {
                    context.Response.StatusCode = 204;
                    context.Response.Close();
                    break;
                }
            }
        }
    }

    private static void OpenBrowser() {
        var scopes = (ScopesService)Services.Get(ServiceId.Scopes);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                         {
                                             FileName = BuildAuthorizationUrl(
                                                                              Constants.ClientId,
                                                                              Constants.RedirectUri,
                                                                              scopes.GetScopesString()
                                                                             ),
                                             UseShellExecute = true,
                                         });
    }

    private static async Task SendLandingPage(HttpListenerContext context) {
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
    
    private static string BuildAuthorizationUrl(string clientId, string redirect, string scopes) {
        var url = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&redirect_uri={redirect}&response_type=token&scope={scopes}&force_verify=true";
        return url;
    }
}