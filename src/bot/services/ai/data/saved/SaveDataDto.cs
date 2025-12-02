using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.ai.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<AiKind> AiKind = new SafeField<AiKind>(ai.AiKind.Ollama);

    public readonly SafeField<List<AiData>> AiData = new SafeField<List<AiData>>(
                                                                                    [
                                                                                        new AiData(
                                                                                                   "Empty",
                                                                                                   "Empty",
                                                                                                   "Empty",
                                                                                                   "http://localhost:11434/api/generate",
                                                                                                   "Empty",
                                                                                                   new AiFallback(State.Disabled, ai.AiKind.Ollama)
                                                                                                  ),
                                                                                        new AiData(
                                                                                                   "Empty",
                                                                                                   "Empty",
                                                                                                   "Empty",
                                                                                                   "https://router.huggingface.co/Empty/v1/chat/completions",
                                                                                                   "Empty",
                                                                                                   new AiFallback(State.Disabled, ai.AiKind.Ollama)
                                                                                                  ),
                                                                                        new AiData(
                                                                                                   "Empty",
                                                                                                   "deepseek-chat",
                                                                                                   "Empty",
                                                                                                   "https://api.deepseek.com/chat/completions",
                                                                                                   "Empty",
                                                                                                   new AiFallback(State.Disabled, ai.AiKind.Ollama)
                                                                                                  ),
                                                                                        new AiData(
                                                                                                   "Empty",
                                                                                                   "publishers/google/models/gemini-pro",
                                                                                                   "Empty",
                                                                                                   "https://aiplatform.googleapis.com/v1/projects/{projectId}/locations/{location}/{model}:generateContent\"",
                                                                                                   "Empty",
                                                                                                   new AiFallback(State.Disabled, ai.AiKind.Ollama)
                                                                                                  ),
                                                                                    ]
                                                                                   );

    public readonly SafeField<string> GoogleProjectId = new SafeField<string>(string.Empty);
    
    public readonly SafeField<long> RemoveChatIn = new SafeField<long>(3600);
    
    
    public SaveDataDto(
        State serviceState,
        AiKind aiKind,
        List<AiData> aiData,
        string googleProjectId,
        long removeChatIn) {
        ServiceState.Value = serviceState;
        AiKind.Value = aiKind;
        AiData.Value = aiData;
        GoogleProjectId.Value = googleProjectId;
        RemoveChatIn.Value = removeChatIn;
    }

    public SaveDataDto() { }
}