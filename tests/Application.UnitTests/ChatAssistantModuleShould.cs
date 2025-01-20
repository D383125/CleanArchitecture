using Application.Common.Interface;
using Application.Dto;
using Application.Interfaces;
using Application.Mapping;
using Application.Modules;
using Application.UnitTests.MockServices;
using AutoMapper;
using Domain.Entities;
using Domain.Events;
using Infrastructure;
using Infrastructure.Services;
using Moq;
using StackExchange.Redis;

namespace Application.UnitTests
{

    public class ChatAssistantModuleShould
    {
        //Move to TextContext
        private readonly IMapper _mapper;
        private readonly IRedisPublisher _redisPublisher;
        private readonly IRedisSubscriber _redisSubscriber;

        public ChatAssistantModuleShould()
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RequestMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            var subscriberMock = new Mock<ISubscriber>();
            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            connectionMultiplexerMock
                .Setup(m => m.GetSubscriber(It.IsAny<object>()))
                .Returns(subscriberMock.Object);
            _redisPublisher = new RedisPublisher(connectionMultiplexerMock.Object);
            _redisSubscriber = new RedisSubscriber(connectionMultiplexerMock.Object);
        }                

        [Fact]        
        public async Task ReturnConversationFromStreamChatCompletion()
        {            
            IChatClient mockChatClient = new MockChatClient();
            var context = new TestContext();
            var repositoryMock = context.CreateService<IGenericRepository<Chat>>();
            var sut = new ChatAssistantModule(repositoryMock, mockChatClient, _mapper, _redisPublisher);

            List<string> results = [];
            await foreach(var chunk in sut.StreamChatCompletionAsync([], string.Empty))
            {
                results.Add(chunk);
            }

            Assert.Equal(new[] { "Hello", "How are you?" }, results);  
        }

        [Fact]
        public async Task CreateNewChatConversationsSucessfully()
        {
            var context = new TestContext();
            await context.InitializeAsync();
            IChatClient mockChatClient = new MockChatClient();
            var repositoryMock = context.CreateService<GenericRepository<Chat>>(context.DbContext);
            var sut = new ChatAssistantModule(repositoryMock, mockChatClient, _mapper, _redisPublisher);
            var chatRequest = new ChatDto
            {
                Message = "[{\"id\": 1, \"role\": \"user\", \"content\": \"Hello Again\"}, {\"id\": 2, \"role\": \"assistant\", \"content\": \"Hi there!\"}, {\"id\": 3, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 4, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}, {\"id\": 5, \"role\": \"user\", \"content\": \"Yello\"}, {\"id\": 6, \"role\": \"assistant\", \"content\": \"Hello! How can I assist you today?\"}, {\"id\": 7, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 8, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}]",
                CreatorId = 1,
                CreatedOn = DateTime.UtcNow,
                LastModifiedOn = DateTime.UtcNow,
            };

            await _redisSubscriber.Subscribe<ChatSavedEvent>(Enum.GetName(Channel.ChatSaved)!, t =>
            {
                Assert.NotEqual(0, t.ChatId);
            });
            await sut.SaveChatAsync(chatRequest, CancellationToken.None);
            
            var created = context.DbContext.ChatHistory.Where(c => c.CreatorId == chatRequest.CreatorId)
                .Select(c => c)
                .FirstOrDefault();
            Assert.Equal(created.CreatorId, chatRequest.CreatorId);
            Assert.True(created.Id != default);
        }        

        [Fact]
        public async Task SaveExistingChatSuccessfully()
        {
            await Task.Yield();
        }
    }    
}

