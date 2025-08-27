using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Models.Product
{
    public class ProductIndexVM
    {
        public IEnumerable<ProductListItemVM> Items { get; set; } = Enumerable.Empty<ProductListItemVM>();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public int? CategoryId { get; set; }      // filter value
        public string? Q { get; set; }
    }
}
