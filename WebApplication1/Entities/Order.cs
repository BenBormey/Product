using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("orders")]
    public class Order
    {
        [Column("id")] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("total_price")] public decimal TotalPrice { get; set; }
        [Column("order_date")] public DateTime? OrderDate { get; set; }
        [Column("status")] public string? Status { get; set; }  = "Peding";

        public User User { get; set; } = default!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }


}