using ChatBot.shared.interfaces;

namespace ChatBot.Services.message_randomizer;

public class SaveData {
    public int counterMax;
    public State serviceState;
    public State loggerState;
    public State randomness;
    public int spreadingFrom;
    public int spreadingTo;
    public MessageState messageState;
    public Message lastGeneratedMessage;
    public readonly List<Message> logs;
    
    
    public SaveData(int counterMax, State serviceState, State loggerState, State randomness, int spreadingFrom, int spreadingTo, MessageState messageState, Message lastGeneratedMessage, List<Message> logs) {
        this.counterMax = counterMax;
        this.serviceState = serviceState;
        this.loggerState = loggerState;
        this.randomness = randomness;
        this.spreadingFrom = spreadingFrom;
        this.spreadingTo = spreadingTo;
        this.messageState = messageState;
        this.lastGeneratedMessage = lastGeneratedMessage;
        this.logs = logs;
    }
}
