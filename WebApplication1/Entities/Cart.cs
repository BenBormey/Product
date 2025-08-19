using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("cart")]
    public class Cart
    {
        [Column("id")] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("Create_At")] public DateTime? CreateAt { get; set; }

        public User User { get; set; } = default!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}