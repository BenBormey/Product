using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CreateProductVM
    {
        [Required, MaxLength(250)]
    public string Name { get; set; } = default!;

    [Range(0, 999999)]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Category")]
    [Required]
    public int CategoryId { get; set; }

    [Display(Name = "Barcode / Code")]
    public string? CodeOrBarcode { get; set; }

    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }

    // for the dropdown
    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
