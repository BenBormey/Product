using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Models
{
    public class ProductIndexVM
    {
        public IEnumerable<WebApplication1.Entities.Product> Items { get; set; } = Enumerable.Empty<WebApplication1.Entities.Product>();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public int? CategoryId { get; set; }      // filter value
        public string? Q { get; set; }
    }
}
