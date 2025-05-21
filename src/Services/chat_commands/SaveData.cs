using ChatBot.shared.interfaces;

namespace ChatBot.Services.chat_commands;

public class SaveData {
    public State serviceState;
    public char commandIdentifier;


    public SaveData(State serviceState, char commandIdentifier) {
        this.serviceState = serviceState;
        this.commandIdentifier = commandIdentifier;
    }
}