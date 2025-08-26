using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.service
{
    public class CheckoutService
    {
        private readonly AppDbContext _db;
        public CheckoutService(AppDbContext db) => _db = db;

        public async Task<CheckoutVM> BuildSummaryAsync(int userId, decimal deliveryFee = 0m, decimal discount = 0m, CancellationToken ct = default)
        {
            var cart = await _db.Set<Cart>()
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null || cart.CartItems.Count == 0) throw new InvalidOperationException("Cart empty.");

            var lines = cart.CartItems.Select(ci => new CartLine(
                ci.ProductId, ci.Product.Name, ci.Product.Image, ci.Product.Price, ci.Quantity, ci.Product.Price * ci.Quantity));

            var subtotal = lines.Sum(x => x.Subtotal);
            var addr = await _db.Set<ShippingAddress>().FirstOrDefaultAsync(a => a.UserId == userId, ct);
            
            return new CheckoutVM(lines, subtotal, deliveryFee, discount, subtotal + deliveryFee - discount, addr);
        }

        public async Task<int> PlaceOrderAsync(int userId, ShippingAddress address, decimal deliveryFee = 0m, decimal discount = 0m, CancellationToken ct = default)
        {
                await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {



                // 1) Load cart with prices
                var cart = await _db.Set<Cart>()
                    .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId, ct);
                if (cart is null || cart.CartItems.Count == 0) throw new InvalidOperationException("Cart is empty.");

                // 2) Upsert shipping address (optional)
                var addr = await _db.shippingAddresses.FirstOrDefaultAsync(a => a.UserId == userId, ct);


                if (addr is null)
                {
                    address.UserId = userId; // ensure user association
                    _db.Add(address);
                }
                else
                {
                    addr.RecipientName = address.RecipientName;
                    addr.Address = address.Address;
             
                }




                // check stock 

                foreach (var iteam in cart.CartItems)
                {
                    var productlist  = await _db.products.Where(p=>p.Id == iteam.ProductId).FirstOrDefaultAsync();
                    if (productlist.QtyInStock is null) throw new InvalidOperationException("Stock not found for product.");
                    if (productlist.QtyInStock < iteam.Quantity) throw new InvalidOperationException("Insufficient stock for product.");
                }


            await _db.SaveChangesAsync(ct);

                // 3) Create order
                var subtotal = cart.CartItems.Sum(i => i.Product.Price * i.Quantity);
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    TotalPrice = subtotal + deliveryFee - discount
                };
                _db.Add(order);
                await _db.SaveChangesAsync(ct);

               

                foreach (var i in cart.CartItems)
                    _db.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = i.ProductId,
                        Qty = i.Quantity,
                        Price = i.Price
                        

                    });
                await _db.SaveChangesAsync(ct);

                // assume cart already loaded with CartItems (and Quantity, ProductId)
                foreach (var i in cart.CartItems)
                {
                    var product = await _db.products
                        .FirstOrDefaultAsync(p => p.Id == i.ProductId, ct);

                    if (product == null) continue; // or throw

                    var current = product.QtyInStock;        // or QtyInstock if that's your prop
                    var newQty = (current ?? 0) - i.Quantity;

                    if (newQty < 0) newQty = 0;              // clamp or validate earlier

                    product.QtyInStock = newQty;             // set back
                }

                await _db.SaveChangesAsync(ct);





                //// update stock 
                //foreach (var item in cart.CartItems)
                //{
                //    var stock = await _db.Set<Stock>()
                //        .FirstOrDefaultAsync(s => s.ProductId == item.ProductId, ct);
                //    if (stock is null) throw new InvalidOperationException("Stock not found for product.");
                //    stock.Qty -= item.Quantity;
                //    if (stock.Qty < 0) throw new InvalidOperationException("Insufficient stock for product.");
                //}

                await _db.SaveChangesAsync(ct);


                // 5) Clear cart
                _db.RemoveRange(cart.CartItems);
                await _db.SaveChangesAsync(ct);

                await tx.CommitAsync(ct);
                return order.Id;
            }catch(Exception ex)
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }

}
