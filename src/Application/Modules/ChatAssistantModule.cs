using Domain.Attributes;
using Application.Common.Interface;
using Domain.Entities;
using Application.Interfaces;
using OpenAI.Chat;

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

        public async Task<IEnumerable<Chat>> GetChatHistoryAsync(CancellationToken cancellationToken)
        {            
            var conversations = await _repository.GetAll();

            return conversations;
        }

        public async Task SaveChatHistoriesAsync(IEnumerable<Chat> chatMessages, CancellationToken cancellationToken)
        {
            //TODO: Map WebApp contracts to domain object. It needs to be written in  as json
           // _repository.UpdateRangeAsync(chatMessages.ToArray());
           return await Task.CompletedTask;
        }
    }
}
