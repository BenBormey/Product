using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("Order_Detail")]
    public class OrderDetail
    {
        [Column("id")] public int Id { get; set; }
        [Column("order_id")] public int OrderId { get; set; }
        [Column("product_id")] public int ProductId { get; set; }
        [Column("qty")] public int Qty { get; set; }
        [Column("price")] public decimal Price { get; set; }

        public Order Order { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public double dis { get; set; }
    }

}