using ChatBot.Services.interfaces;
using ChatBot.Shared.interfaces;

namespace ChatBot.Services.message_randomizer;

public class SaveData {
    public int counterMax;
    public State state;
    public int counter;
    public State randomness;
    public Range spreading;
    public int randomValue;
    public List<Message> logs = [];


    public SaveData() {}
    
    public SaveData(int counterMax, State randomness, Range spreading) {
        this.counterMax = counterMax;
        this.randomness = randomness;
        this.spreading = spreading;
    }
}