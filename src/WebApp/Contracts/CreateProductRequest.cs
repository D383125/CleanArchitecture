using System.ComponentModel.DataAnnotations;

namespace WebApp.Contracts;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; }

    public decimal Price { get; set; }
}