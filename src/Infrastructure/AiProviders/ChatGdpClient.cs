using Application.Common.Interface;
using Domain.Attributes;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace Infrastructure.AiProviders
{
    [DI(typeof(IChatClient))]
    public class ChatGdpClient : IChatClient
    {
        private readonly OpenAIClient _client;
        public ChatGdpClient(IOptions<ApplicationOptions> options)
        {
            _client = new OpenAIClient(options.Value.OpenAIKey); //DI
        }

        public async IAsyncEnumerable<string> CompleteChatStreaming(IDictionary<string, string> messages, string model = "gpt-4")
        {
            var client = _client.GetChatClient(model);

            var nativeMessages = messages.Select<KeyValuePair<string, string>, ChatMessage>
                (kvp => 
            {
                return kvp.Key.ToLower() switch
                {
                    "user" => ChatMessage.CreateUserMessage(kvp.Value),
                    "assistant" => ChatMessage.CreateAssistantMessage(kvp.Value),
                    "system" => ChatMessage.CreateSystemMessage(kvp.Value),
                    _ => throw new ArgumentException($"Invalid role: {kvp.Key}")
                };
            }).ToList();

            System.ClientModel.AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(nativeMessages);

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
