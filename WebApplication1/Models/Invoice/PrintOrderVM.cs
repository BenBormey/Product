using WebApplication1.Entities;

namespace WebApplication1.Models.Invoice
{
    public class PrintOrderVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }   // we'll coalesce from DateTime?
        public List<PrintItemVM> Items { get; set; } = new();

        public decimal Subtotal { get; set; }
        public decimal GrandTotal { get; set; }   // no VAT/fees/discounts for now
        public decimal Paid { get; set; }         // keep if you pass a value
        public decimal Change { get; set; }
        public decimal Due { get; set; }

        public bool IsReceipt { get; set; }
        public  string Phone { get; set; }     

    }
}
