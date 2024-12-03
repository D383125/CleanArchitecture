using Domain.Attributes;
using Application.Common.Interface;

namespace Application.Modules
{
    [DI]
    public sealed class ChatAssistantModule
    {
        private readonly IChatClient _chatClient;
        public ChatAssistantModule(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async IAsyncEnumerable<string> StreamChatCompletionAsync(IDictionary<string, string> messages, string model)
        {            
            var completionUpdates = _chatClient.CompleteChatStreaming(messages, model);

            await foreach (var completionUpdate in completionUpdates)
            {
                if (completionUpdate != null)
                {
                    yield return completionUpdate;
                }
            }         
        }
    }
}
