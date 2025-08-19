
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("categories")]
    public class Category
    {
        [Column("id")] public int Id { get; set; }
        [Column("category_name")] public string? CategoryName { get; set; }
        [Column("description")] public string? Description { get; set; }
        [Column("created_at")] public DateTime? CreatedAt { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("is_active")] public bool? IsActive { get; set; }

        public ICollection<Product> Products { get; set; }
    }

}