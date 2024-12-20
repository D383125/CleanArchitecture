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
        public async Task ReturnConversationFromStreamChatCompletionAsync()
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
        public void UpdateNewAndAmendedChatConversationsSucessfully()
        {

        }

    }    
}

