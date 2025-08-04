namespace ChatBot.services.ai.AiClients.HuggingFace;

public class ChatCompletionResponse
{
    public string? Id { get; set; }
    public List<Choice>? Choices { get; set; }
    public long Created { get; set; }
    public string? Model { get; set; }
    public string? Object { get; set; }
    public string? ServiceTier { get; set; }
    public string? SystemFingerprint { get; set; }
    public Usage? Usage { get; set; }
    public object? PromptLogprobs { get; set; }
}