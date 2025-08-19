using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
  
    public class Stock
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("qty")]
        public int Qty { get; set; }

        [Column("update_at")]
        public DateTime? UpdatedAt { get; set; }

        public Product Product { get; set; } = default!;
    }
}