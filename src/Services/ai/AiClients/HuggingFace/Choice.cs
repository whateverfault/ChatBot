namespace ChatBot.Services.ai.AiClients.HuggingFace;

public class Choice {
    public string? FinishReason { get; set; }
    public int Index { get; set; }
    public object? Logprobs { get; set; }
    public Message? Message { get; set; }
    public int? StopReason { get; set; }
}