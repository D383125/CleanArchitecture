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

        public async IAsyncEnumerable<string> CompleteChatStreaming(IEnumerable<KeyValuePair<string, string>> messages, string model = "gpt-4")
        {
            const int baseIndex = 0;
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

            ChatCompletionOptions options = new()
            {
                Temperature = 0
            };

            System.ClientModel.AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(nativeMessages, options);

            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0 && !string.IsNullOrEmpty(completionUpdate.ContentUpdate[0].Text))
                {
                    yield return completionUpdate.ContentUpdate[baseIndex].Text;
                }
            }
        }
    }
}
