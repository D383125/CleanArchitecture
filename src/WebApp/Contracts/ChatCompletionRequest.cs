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

    //public class ChatHistory
    //{
    //    public ChatMessage[] Messages { get; set; } = [];
    //}
}