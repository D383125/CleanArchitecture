using Application.Common.Interface;
using Application.Dto;
using Application.Interfaces;
using Application.Mapping;
using Application.Modules;
using Application.UnitTests.MockServices;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services;
using Moq;
using StackExchange.Redis;

namespace Application.UnitTests
{
    //TODO: See https://www.youtube.com/watch?v=8IRNC7qZBmk
    //Wire up to docker

    public class ChatAssistantModuleShould
    {
        //Move to TextContext
        private readonly IMapper _mapper;
        private readonly IRedisPublisher _redisPublisher;

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
        }                

        [Fact]        
        public async Task ReturnConversationFromStreamChatCompletion()
        {
            var context = new TestContext();
            IChatClient mockChatClient = new MockChatClient();
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
            //TODO: Add postgtes docker db
            var context = new TestContext();
            IChatClient mockChatClient = new MockChatClient();
            var repositoryMock = context.CreateService<IGenericRepository<Chat>>();
            var sut = new ChatAssistantModule(repositoryMock, mockChatClient, _mapper, _redisPublisher);
            var chatRequest = new ChatDto
            {
                Message = "[{\"id\": 1, \"role\": \"user\", \"content\": \"Hello Again\"}, {\"id\": 2, \"role\": \"assistant\", \"content\": \"Hi there!\"}, {\"id\": 3, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 4, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}, {\"id\": 5, \"role\": \"user\", \"content\": \"Yello\"}, {\"id\": 6, \"role\": \"assistant\", \"content\": \"Hello! How can I assist you today?\"}, {\"id\": 7, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 8, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}]",
                CreatorId = 1,
                CreatedOn = DateTime.UtcNow,
                LastModifiedOn = DateTime.UtcNow,
            };

            await sut.SaveChatAsync(chatRequest, CancellationToken.None);

        }        

        [Fact]
        public async Task SaveExistingChatSuccessfully()
        {
            await Task.Yield();
        }
    }    
}

