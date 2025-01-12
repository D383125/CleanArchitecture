using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration
{
    public class ApplicationOptions
    {
        [Required]
        public required string OpenAIKey { get; set; }
        [Required]
        public required AWSOptions Aws { get; set; }
    }

    public record AWSOptions
    {
        public required string ApiKey { get; set; }
        public required string PKey { get; set; }
    }
}