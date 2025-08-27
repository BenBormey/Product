namespace WebApplication1.Models.CartDTO
{
    public class CartItemVM
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public decimal dis { get; set; } 
        public decimal Price { get; set; }        // unit price (snapshot when added)
        public decimal SubTotal => Quantity * (Price - dis);
    }
}
