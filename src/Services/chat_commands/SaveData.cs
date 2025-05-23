using ChatBot.shared.interfaces;

namespace ChatBot.Services.chat_commands;

public class SaveData {
    public char commandIdentifier;
    public State serviceState;


    public SaveData(State serviceState, char commandIdentifier) {
        this.serviceState = serviceState;
        this.commandIdentifier = commandIdentifier;
    }
}