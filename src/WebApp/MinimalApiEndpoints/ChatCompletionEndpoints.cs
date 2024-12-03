using Application.Modules;
using OpenAI.Chat;
using System.Runtime.InteropServices;
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
                    IDictionary<string, string> nativeMessages = chatRequest.Messages
                        .Select(m => new KeyValuePair<string, string>(m.Role, m.Content))
                        .ToDictionary();
                    await foreach (var chunk in chatAssistant.StreamChatCompletionAsync(nativeMessages, "gpt-4"))
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
