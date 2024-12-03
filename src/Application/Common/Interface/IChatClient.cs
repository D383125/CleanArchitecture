namespace Application.Common.Interface
{
    
    public interface IChatClient
    {
        IAsyncEnumerable<string> CompleteChatStreaming(IDictionary<string, string> messages, string model);
    }
}
