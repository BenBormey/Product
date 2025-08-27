namespace WebApplication1.Models.Invoice
{
    public class PrintItemVM
    {
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public decimal dis { get;set; }
    }
}
