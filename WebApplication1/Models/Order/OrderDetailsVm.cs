using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Order
{
    public class OrderDetailsVm
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalPrice { get; set; }

        public List<OrderItemRow> Items { get; set; } = new();

        public int ItemsCount => Items.Sum(x => x.Qty);
        public decimal Subtotal => Items.Sum(x => x.LineTotal);
         public decimal? cashreturn { get; set; }
        
         public decimal? cashAmount { get; set; }
        //public decimal cashreturn { get; set; }

        // If you don’t store delivery/discount separately, show the net “adjustments”
        public decimal Adjustments => TotalPrice - Subtotal;
    }
}
