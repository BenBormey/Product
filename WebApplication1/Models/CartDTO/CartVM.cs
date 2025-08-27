namespace WebApplication1.Models.CartDTO
{
    public class CartVM
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemVM> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.SubTotal);
    }
}
