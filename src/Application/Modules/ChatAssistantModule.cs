using Domain.Attributes;
using Application.Common.Interface;
using Domain.Entities;

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

        public async IAsyncEnumerable<string> StreamChatCompletionAsync(IEnumerable<KeyValuePair<string, string>> messages, string model)
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

        //TODO: Wednesday 1. Connect to db. Use simple accessor (intially) ot move to commandhandler.
        //2. Publish images to docker reposoitry
        //3. dontnet, ract arch and sql questiona
        //public IEnumerable<Chat> GetChatHistory(CancellationToken cancellationToken)
        //{
        //    return 

        //}
    }
}
