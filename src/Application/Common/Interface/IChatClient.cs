namespace Application.Common.Interface
{

    public interface IChatClient
    {
        IAsyncEnumerable<string> CompleteChatStreaming(IEnumerable<KeyValuePair<string, string>> messages, string model);
    }
}
