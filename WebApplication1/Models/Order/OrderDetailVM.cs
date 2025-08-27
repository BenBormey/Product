using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Order
{
    public class OrderDetailVM
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 999)]
        public int Qty { get; set; } = 1;

        // optional: where to redirect back to
        public string? ReturnUrl { get; set; }
    }
}
