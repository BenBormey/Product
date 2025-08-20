using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("shipping_addresses")]
    public class ShippingAddress
    {
        [Column("id")] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("recipient_name")] public string RecipientName { get; set; }
        [Column("address")] public string Address { get; set; } = default!;
        [Column("city")] public string City { get; set; } = default!;
        [Column("zip_code")] public string? ZipCode { get; set; }
        [Column("country")] public string? Country { get; set; }

        public User User { get; set; } = default!;
    }
}