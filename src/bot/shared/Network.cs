using System.Net;

namespace ChatBot.bot.shared;

public static class Network {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                                                                  PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                                                                  MaxConnectionsPerServer = 50,
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                                  UseProxy = true,
                                                                  Proxy = WebRequest.DefaultWebProxy,
                                                              };

    public static readonly HttpClient HttpClient;


    static Network() {
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        
        HttpClient = new HttpClient(_httpHandler);
        HttpClient.Timeout = TimeSpan.FromSeconds(30);
    }
}