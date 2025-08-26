namespace WebApplication1.Models
{
    public class ProductListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Image { get; set; }
        public string CategoryName { get; set; } = "-";
        public decimal Price { get; set; }
        public double DiscountPercent { get; set; }   // matches FLOAT in DB
        public decimal FinalPrice { get; set; }
        public bool IsOnSale { get; set; }
        public int QtyInStock { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
