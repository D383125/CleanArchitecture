using Application.Modules;
using OpenAI.Chat;
using WebApp.Contracts;

namespace WebApp.MinimalApiEndpoints
{
    internal static class ChatCompletionEndpoints
    {
        internal static void MapChatEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/chat", async (ChatAssistantModule chatAssistant, ChatCompletionRequest chatRequest, HttpContext context) =>
            {
                try
                {
                    context.Response.ContentType = "text/plain";
                    
                    var nativeMessages = chatRequest.Messages.Select<ChatCompletionMessage, ChatMessage>
                    (m =>
                    {
                        return m.Role.ToLower() switch
                        {
                            "user" => ChatMessage.CreateUserMessage(m.Content),
                            "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
                            "system" => ChatMessage.CreateSystemMessage(m.Content),
                            _ => throw new ArgumentException($"Invalid role: {m.Role}")
                        };
                    }).ToList();

                    await foreach (var chunk in chatAssistant.StreamChatCompletionAsync(nativeMessages, "o1-mini"))
                    {
                        await context.Response.WriteAsync(chunk);
                        await context.Response.Body.FlushAsync(); // Ensure chunks are sent immediately
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Error: {ex.Message}");
                }
            })
            .WithName("CreateChat")
            .WithTags("Chat bot")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
