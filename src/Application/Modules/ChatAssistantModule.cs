using Application.Common.Interface;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Attributes;
using Domain.Entities;
using Domain.Events;

namespace Application.Modules
{
    [DI]
    public sealed class ChatAssistantModule
    {
        private readonly IGenericRepository<Chat> _repository;
        private readonly IChatClient _chatClient;
        private readonly IMapper _mapper;
        private readonly IRedisPublisher _redisPublisher;

        public ChatAssistantModule(IGenericRepository<Chat> repository, IChatClient chatClient, IMapper mapper, IRedisPublisher redisPublisher)
        {
            _repository = repository;
            _chatClient = chatClient;
            _mapper = mapper;
            _redisPublisher = redisPublisher;
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
            cancellationToken.ThrowIfCancellationRequested();
            var conversations = await _repository.GetAll();

            return conversations;
        }

        public async Task SaveChatAsync(ChatDto chatRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chatRequest, nameof(chatRequest));
            cancellationToken.ThrowIfCancellationRequested();

            System.Diagnostics.Trace.TraceInformation($"Saving {chatRequest}");
            Chat entity = _mapper.Map<Chat>(chatRequest);
            await _repository.AddOrUpdateAsync(entity);
            await _repository.CommitAsync(cancellationToken);
            await _redisPublisher.PublishAsync(Channel.ChatSaved, new ChatSavedEvent(entity.Id));
        }
    }
}
