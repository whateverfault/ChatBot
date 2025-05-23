using ChatBot.shared.interfaces;

namespace ChatBot.Services.message_randomizer;

public class SaveData {
    public readonly List<Message> logs;
    public int counterMax;
    public Message lastGeneratedMessage;
    public State loggerState;
    public MessageState messageState;
    public State randomness;
    public State serviceState;
    public int spreadingFrom;
    public int spreadingTo;


    public SaveData(int counterMax, State serviceState, State loggerState, State randomness, int spreadingFrom, int spreadingTo,
                    MessageState messageState, Message lastGeneratedMessage, List<Message> logs) {
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