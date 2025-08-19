namespace WebApplication1.Models
{
    public class CartVM
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemVM> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.SubTotal);
    }
}
