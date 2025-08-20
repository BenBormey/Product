using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.service
{
    public interface IPaymentService
    {
     
        string GenerateQrFor(Order order);

     
        Task<bool> Verify(Order order);

    
        bool ValidateSignature(string provider, IHeaderDictionary headers, PaymentCallback payload);
    }

}
