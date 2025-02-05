using Application.Common.Interface;

namespace Application.UnitTests.MockServices
{
    public class MockChatClient : IChatClient
    {
        async IAsyncEnumerable<string> IChatClient.CompleteChatStreaming(IEnumerable<KeyValuePair<string, string>> messages, string model)
        {
            await Task.Yield();

            yield return "Hello";
            yield return "How are you?";
        }
    }
}
