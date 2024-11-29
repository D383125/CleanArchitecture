using Domain.Attributes;
using OpenAI.Chat;
using OpenAI;
using System.ClientModel;


namespace Application.Modules
{
    [DI]
    public sealed class ChatAssistantModule
    {
        private readonly OpenAIClient _client;        
        public ChatAssistantModule(OpenAIClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<string> StreamChatCompletionAsync(List<ChatMessage> messages, string model = "gpt-4")
        {
            var client = _client.GetChatClient(model);

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(messages);
            
            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    yield return completionUpdate.ContentUpdate[0].Text;                    
                }
            }
        }
    }
}
