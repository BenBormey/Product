using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ProductEditVM
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = default!;

        [Range(0, 100000)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        // keep old image url/path
        public string? Image { get; set; }

        // new upload (optional)
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // for dropdown
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
