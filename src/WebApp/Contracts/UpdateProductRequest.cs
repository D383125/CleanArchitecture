using System.ComponentModel.DataAnnotations;

namespace WebApp.Contracts;

public class UpdateProductRequest
{
    [Required]
    public string Name { get; set; }

    public decimal Price { get; set; }
}