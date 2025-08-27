using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public record CheckoutVM(
      IEnumerable<CartLine> Lines,
      decimal Subtotal,
      decimal DeliveryFee,
      decimal Discount,
      decimal Total,
      ShippingAddress Address
  );
    public record CartLine(int ProductId, string Name, string? Image, decimal Price, int Qty, decimal Subtotal,decimal discount);

}
