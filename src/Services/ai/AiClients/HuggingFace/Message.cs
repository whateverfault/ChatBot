namespace ChatBot.services.ai.AiClients.HuggingFace;

public class Message {
    public string? Content { get; set; }
    public object? Refusal { get; set; }
    public string? Role { get; set; }
    public object? Audio { get; set; }
    public object? FunctionCall { get; set; }
    public List<object>? ToolCalls { get; set; }
    public object? ReasoningContent { get; set; }
}