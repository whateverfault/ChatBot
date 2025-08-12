namespace ChatBot.bot.services.chat_logs;

public class Message {


    public Message(string msg, string username) {
        Msg = msg;
        Username = username;
    }

    public string Msg { get; }
    public string Username { get; }
}