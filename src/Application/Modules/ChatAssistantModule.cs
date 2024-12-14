using Domain.Attributes;
using Application.Common.Interface;
using Domain.Entities;
using Application.Interfaces;

namespace Application.Modules
{
    [DI]
    public sealed class ChatAssistantModule
    {
        private readonly IGenericRepository<Chat> _repository;
        private readonly IChatClient _chatClient;
        public ChatAssistantModule(IGenericRepository<Chat> repository, IChatClient chatClient)
        {
            _repository = repository;
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

        //TODO: 
        //2. Publish images to docker reposoitry
        //3. dontnet, ract arch and sql questions
        public async Task<IEnumerable<Chat>> GetChatHistory(CancellationToken cancellationToken)
        {            
            var conversations = await _repository.GetAll();

            return conversations;
        }
    }
}
