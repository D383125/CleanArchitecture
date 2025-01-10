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
            ArgumentNullException.ThrowIfNull(nameof(messages));

            var completionUpdates = _chatClient.CompleteChatStreaming(messages, model);

            await foreach (var completionUpdate in completionUpdates)
            {
                if (completionUpdate != null)
                {
                    yield return completionUpdate;
                }
            }         
        }

        public async Task<IEnumerable<Chat>> GetChatAsync(CancellationToken cancellationToken)
        {            
            var conversations = await _repository.GetAll();

            return conversations;
        }

        public async Task SaveChatAsync(Chat chatRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chatRequest, nameof(chatRequest));

            System.Diagnostics.Trace.TraceInformation($"Saving {chatRequest}");
            await _repository.AddOrUpdateAsync(chatRequest);            
            await _repository.CommitAsync(cancellationToken);           
        }
    }
}
