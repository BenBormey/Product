using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Entities
{
    [Table("promotions")]
    public class Promotion
    {
        [Column("id")] public int Id { get; set; }
        [Column("product_id")] public int ProductId { get; set; }
        [Column("discount_percent")] public decimal DiscountPercent { get; set; }
        [Column("start_date")] public DateTime StartDate { get; set; }
        [Column("end_date")] public DateTime EndDate { get; set; }
       

        public Product Product { get; set; } = default!;
    }
}