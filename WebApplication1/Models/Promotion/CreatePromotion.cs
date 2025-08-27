namespace WebApplication1.Models.Promotion
{
    public class CreatePromotion
    {
        public int ProductId { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
