using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = default!;

        [Column("email")]
        public string Email { get; set; } = default!;

        [Column("password")]
        public string Password { get; set; } = default!;

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("RegisteredDate")]
        public DateTime? RegisteredDate { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        // nav
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<ShippingAddress> shipping_address { get; set; } = new List<ShippingAddress>();
        public ICollection<Order> orders { get; set; }
        public ICollection<Review> reviews { get; set; }
    }


}