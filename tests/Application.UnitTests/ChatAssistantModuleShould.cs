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
                                    
            var anyParms = new Dictionary<string, string>();
            anyParms.Add("User", "Hello");

            List<string> results = new List<string>();
            await foreach(var chunk in sut.StreamChatCompletionAsync(anyParms, string.Empty))
            {
                results.Add(chunk);
            }

            Assert.Equal(new[] { "Hello", "How are you?" }, results);  
        }        
    }    
}

