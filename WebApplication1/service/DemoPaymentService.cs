using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.service
{
    public class DemoPaymentService : IPaymentService
    {
        public string GenerateQrFor(Order order)
        {
            var fakeData = $"Order:{order.Id}-Amount:{order.TotalPrice}";
            return $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={Uri.EscapeDataString(fakeData)}";

        }

        public bool ValidateSignature(string provider, IHeaderDictionary headers, PaymentCallback payload)
        {
            return true; //
        }

        public async  Task<bool> Verify(Order order)
        {
            // 👉 Normally call provider API: check status by transactionId
            await Task.Delay(200); // simulate
            return true; // fake "paid"
        }
    }
}
