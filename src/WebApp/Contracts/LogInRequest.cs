
namespace WebApp.Contracts
{
    public record class LogInRequest
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}
