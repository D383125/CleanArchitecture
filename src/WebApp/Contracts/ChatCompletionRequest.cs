namespace WebApp.Contracts
{
    //Post
    public class ChatCompletionRequest
    {
        public string Model { get; set; } = "gpt-4o"; // Default value, can be overridden
        public List<ChatCompletionMessage> Messages { get; set; } = [];
    }

    public class ChatCompletionMessage
    {
        public string Role { get; set; } // Values: "system", "user", "assistant"
        public string Content { get; set; }
    }

    //Put
    public class ChatMessage : ChatCompletionMessage
    {
        public int Id { get; set; }
    }

    public record ChatRequest
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public required string Message { get; set; }
    }
}