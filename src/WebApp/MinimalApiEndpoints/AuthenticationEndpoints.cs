using WebApp.Contracts;
using Infrastructure;
using System.Security.Claims;

namespace WebApp.MinimalApiEndpoints
{
    internal static class AuthenticationEndpoints
    {
        internal static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("identity", (ClaimsPrincipal user) => user.Claims.Select(c => new { c.Type, c.Value }))
                .RequireAuthorization();

            app.MapPost("authenticate", async (
                LogInRequest request,
                ApplicationDbContext context,
                CancellationToken ct) =>
            {                
                throw new NotImplementedException();
            })
            .WithName("Login")
            .WithTags("Authenticate")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
