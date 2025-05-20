namespace ChatBot.Services.message_randomizer;

public class Message {
    public string Msg { get; }
    public string Username { get; }
    
    
    public Message(string msg, string username) {
        Msg = msg;
        Username = username;
    }
}