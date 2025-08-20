    using QRCoder;
using WebApplication1.Repository;
namespace WebApplication1.service
{
  
    // Services/Payments/QrService.cs


        public class QrService : IQrService
        {
        public string MakeQr(string text)
            => "data:image/svg+xml;base64," +
               Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                   $"<svg xmlns='http://www.w3.org/2000/svg' width='256' height='256'><rect width='100%' height='100%' fill='black'/><text x='50%' y='50%' dominant-baseline='middle' text-anchor='middle' fill='white' font-size='18'>QR</text></svg>"
               ));

        public string MakeQrForAmount(decimal amount, string? payload = null)
            => MakeQr(payload ?? $"AMT={amount:F2}");
    }
    

}
