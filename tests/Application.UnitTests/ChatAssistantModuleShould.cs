using Application.Common.Interface;
using Application.Modules;
using Application.UnitTests.MockServices;

namespace Application.UnitTests
{
    public class ChatAssistantModuleShould
    {
        [Fact]        
        public async Task ReturnConversationFromStreamChatCompletionAsync()
        {            
            IChatClient mockChatClient = new MockChatClient();
            var sut = new ChatAssistantModule(mockChatClient);                                                

            List<string> results = new();
            await foreach(var chunk in sut.StreamChatCompletionAsync(new Dictionary<string, string>(), string.Empty))
            {
                results.Add(chunk);
            }

            Assert.Equal(new[] { "Hello", "How are you?" }, results);  
        }        
    }    
}

