using Application.Modules;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using WebApp.Contracts;

namespace WebApp.MinimalApiEndpoints
{
    internal static class ChatCompletionEndpoints
    {
        //TODO: Map Group to map all endpoints

        internal static void MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            //TODO:
            //2. Persist chats (add endpoints) With Save button
            //3. Train Endpoint
            app.MapPut("/chat", async (ChatAssistantModule chatAssistant, ChatMessage[] chatRequest, CancellationToken ct) => {

                var chatMessages = chatRequest.Select(m => new Chat
                {
                    Message = m.Content,
                    LastModifiedOn = DateTimeOffset.UtcNow,
                });

                await chatAssistant.SaveChatHistoriesAsync(chatMessages, ct);
                 
            })
            .WithName("SaveChats")
            .WithTags("Chat bot")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

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

            app.MapGet("/chat", async (
                ChatAssistantModule chatAssistant, 
                IDistributedCache cache, 
                HttpContext context, 
                CancellationToken ct) =>
            {
            var chatHistory = await cache.GetAsync("allChatHistory", 
                async token => await chatAssistant.GetChatHistoryAsync(ct),
                    CacheOptions.DefaultExpiration,
                    ct);

            return chatHistory is null ? Results.NotFound() : Results.Ok(chatHistory);


        })
            .WithName("Get Chat History")
            .WithTags("Chat bot")
            .Produces<IEnumerable<Chat>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
