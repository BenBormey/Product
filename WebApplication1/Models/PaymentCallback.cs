namespace WebApplication1.Models
{
    public class PaymentCallback
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string RawSignature { get; set; } = string.Empty;
    }
}
