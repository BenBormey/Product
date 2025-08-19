using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;

public class CartService
{
    private readonly AppDbContext _db;
    public CartService(AppDbContext db) => _db = db;

    public async Task<Cart> GetOrCreateAsync(int userId, CancellationToken ct = default)
    {
        var cart = await _db.carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null)
        {
            cart = new Cart { UserId = userId, CreateAt = DateTime.Now };
            _db.carts.Add(cart);
            await _db.SaveChangesAsync(ct);
            cart.CartItems = new List<CartItem>();
        }
        return cart;
    }
}
