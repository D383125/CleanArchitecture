using Domain.Entities;
using Domain;
using WebApp.Contracts;

namespace WebApp.MinimalApiEndpoints
{
    internal static class AuthenticationEndpoints
    {
        internal static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("authenticate", async (
                LogInRequest request,
                ApplicationDbContext context,
                CancellationToken ct) =>
            {
                //var product = new Product
                //{
                //    Name = request.Name,
                //    Price = request.Price
                //};

                //context.Add(product);

                //await context.SaveChangesAsync(ct);

                //return Results.Ok(product);
                throw new NotImplementedException();
            })
            .WithName("Login")
            .WithTags("Authenticate")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
