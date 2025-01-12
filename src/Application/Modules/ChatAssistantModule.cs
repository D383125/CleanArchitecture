using Domain.Attributes;
using Application.Common.Interface;
using Domain.Entities;
using Application.Interfaces;
using Application.Dto;
using AutoMapper;

namespace Application.Modules
{
    [DI]
    public sealed class ChatAssistantModule
    {
        private readonly IGenericRepository<Chat> _repository;
        private readonly IChatClient _chatClient;
        private readonly IMapper _mapper;

        public ChatAssistantModule(IGenericRepository<Chat> repository, IChatClient chatClient, IMapper mapper)
        {
            _repository = repository;
            _chatClient = chatClient;
            _mapper = mapper;
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

        public async Task SaveChatAsync(ChatDto chatRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chatRequest, nameof(chatRequest));

            System.Diagnostics.Trace.TraceInformation($"Saving {chatRequest}");
            Chat entity = _mapper.Map<Chat>(chatRequest);
            await _repository.AddOrUpdateAsync(entity);
            await _repository.CommitAsync(cancellationToken);           
        }
    }
}
