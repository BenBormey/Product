namespace WebApplication1.Models.Promotion
{
    public class PromotionList
    {
        public int PromotionId { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // product info
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal ProductPrice { get; set; }
        public string? ProductImage { get; set; }


    }
}
