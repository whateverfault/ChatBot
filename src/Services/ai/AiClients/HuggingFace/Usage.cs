namespace ChatBot.services.ai.AiClients.HuggingFace;

public class Usage {
    public int CompletionTokens { get; set; }
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
    public object? CompletionTokensDetails { get; set; }
    public object? PromptTokensDetails { get; set; }
}