using Application.Common.Interface;
using Application.Interfaces;
using Application.Modules;
using Application.UnitTests.MockServices;
using Domain.Entities;
using Moq;

namespace Application.UnitTests
{
    public class ChatAssistantModuleShould
    {
        [Fact]        
        public async Task ReturnConversationFromStreamChatCompletion()
        {            
            IChatClient mockChatClient = new MockChatClient();
            var repositoryMock = new Mock<IGenericRepository<Chat>>(); // Configure
            var sut = new ChatAssistantModule(repositoryMock.Object, mockChatClient);

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
            //TODO: Add postgtee docker db
            IChatClient mockChatClient = new MockChatClient();
            var repositoryMock = new Mock<IGenericRepository<Chat>>(); // Configure
            var sut = new ChatAssistantModule(repositoryMock.Object, mockChatClient);
            var chatRequest = new Chat
            {
                Message = "[{\"id\": 1, \"role\": \"user\", \"content\": \"Hello Again\"}, {\"id\": 2, \"role\": \"assistant\", \"content\": \"Hi there!\"}, {\"id\": 3, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 4, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}, {\"id\": 5, \"role\": \"user\", \"content\": \"Yello\"}, {\"id\": 6, \"role\": \"assistant\", \"content\": \"Hello! How can I assist you today?\"}, {\"id\": 7, \"role\": \"user\", \"content\": \"what planet am i on\"}, {\"id\": 8, \"role\": \"assistant\", \"content\": \"You are on planet Earth.\"}]",
                CreatorId = 1,
                CreatedOn = DateTime.UtcNow,
                LastModifiedOn = DateTime.UtcNow,
            };

            await sut.SaveChatAsync(chatRequest, CancellationToken.None);

        }

        [Fact]
        public async Task UpdateExistingChatSuccessfully()
        {

        }

    }    
}

