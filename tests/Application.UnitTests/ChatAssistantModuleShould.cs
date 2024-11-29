using Application.Modules;
using Moq;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Threading.Channels;

namespace Application.UnitTests
{
    public class ChatAssistantModuleShould
    {
        
            [Fact]        
            public async Task StreamChatCompletionAsync_ShouldThrowNullException()
            {
            
                var mockChatClient = new Mock<ChatClient>();
                var mockOpenAiClient = new Mock<OpenAIClient>();

            // Simulating the behavior of CompleteChatStreamingAsync to return predefined updates.
            mockChatClient
                .Setup(c => c.CompleteChatStreamingAsync(It.IsAny<ChatMessage>()));
                //.Returns(); //TODO: Need to simulate repsone but its cloesed. USe adaptor?

            mockOpenAiClient
                .Setup(factory => factory.GetChatClient(It.IsAny<string>()))
                .Returns(mockChatClient.Object);

            var chatService = new ChatAssistantModule(mockOpenAiClient.Object);
            var userMessage = ChatMessage.CreateUserMessage("Hello");
            mockOpenAiClient.Setup(c => c.GetChatClient(It.IsAny<string>())).Returns(mockChatClient.Object);            
            
            // Act
            var results = new List<string>();
            //Assert.Throws<NullReferenceException>(() => chatService.StreamChatCompletionAsync(new List<ChatMessage> { userMessage }));
            //await foreach (var result in chatService.StreamChatCompletionAsync(new List<ChatMessage> { userMessage }))
            //{
            //    results.Add(result);
            //}
            //);
            var result = chatService.StreamChatCompletionAsync(new List<ChatMessage> { userMessage });

            // Assert
            //Assert.Equal(new[] { "Hello", "World" }, results);
            //Assert.Equal(result.GetAsyncEnumerator())

        }        
    }    
}

