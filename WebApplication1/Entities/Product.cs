using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("products")]
    public class Product
{
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; } = default!;
        [Column("price")] public decimal Price { get; set; }
        [Column("description")] public string? Description { get; set; }
        [Column("image")] public string? Image { get; set; }
        [Column("Category_ID")] public int CategoryId { get; set; }
        [Column("CreatedDate")] public DateTime? CreatedDate { get; set; }
        [Column("Code_or_barcode")] public string? CodeOrBarcode { get; set; }
        [Column("QtyInstock")] 
        public int? QtyInStock { get; set; }
        public Category? Category { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } 
    public ICollection<CartItem> CartItems { get; set; } 
    public ICollection<Review> Reviews { get; set; } 
}
}