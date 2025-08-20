namespace WebApplication1.Models.Order
{
    public class OrderItemRow
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal LineTotal { get; set; }
    }
}
