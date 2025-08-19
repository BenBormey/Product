using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Entities
{
    [Table("cartItems")]
    public class CartItem
    {
        [Column("id")] public int Id { get; set; }
        [Column("cart_id")] public int CartId { get; set; }
        [Column("product_id")] public int ProductId { get; set; }
        [Column("quantity")] public int Quantity { get; set; }
        [Column("price")] public decimal Price { get; set; }
        public Cart Cart { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}