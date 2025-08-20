namespace WebApplication1.Models.Dashboard
{
    public class TrendingItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public TrendingItemDto() { }
        public TrendingItemDto(int productId, string name, decimal price, string? imageUrl = null)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            ImageUrl = imageUrl;
        }
    }
}
