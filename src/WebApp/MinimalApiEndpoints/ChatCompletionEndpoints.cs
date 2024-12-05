using Application.Modules;
using WebApp.Contracts;

namespace WebApp.MinimalApiEndpoints
{
    internal static class ChatCompletionEndpoints
    {
        internal static void MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            //TODO:
            //1. Add resids cache check
            //2. Persist chats (add endpoints) With Save button
            //3. Train Endpoint
            app.MapPost("/chat", async (ChatAssistantModule chatAssistant, ChatCompletionRequest chatRequest, HttpContext context) =>
            {
                try
                {
                    context.Response.ContentType = "text/plain";
                    IEnumerable<KeyValuePair<string, string>> nativeMessages = chatRequest.Messages
                        .Select(m => new KeyValuePair<string, string>(m.Role, m.Content));
                  
                    await foreach (var chunk in chatAssistant.StreamChatCompletionAsync(nativeMessages, chatRequest.Model))
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
