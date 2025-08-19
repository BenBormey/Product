using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models; // ប្តូរប្រសិនបើ namespace ផ្សេង
[Authorize]
public class CartController : Controller
{


    private readonly AppDbContext _db;
    private readonly CartService _cartService;

    public CartController(AppDbContext db)
    {
        _db = db;
        _cartService = new CartService(db);
    }

    // GET: /Cart
    public async Task<IActionResult> Index()
    {
        var userId = 1; // TODO: replace with real logged-in user id

        var cart = await _db.carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            return View(new CartVM { CartId = 0, UserId = userId, Items = new() });
        }

        var vm = new CartVM
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(i => new CartItemVM
            {
                CartItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Image = string.IsNullOrWhiteSpace(i.Product.Image) ? "/images/placeholder-food.jpg" : i.Product.Image,
                Quantity = i.Quantity,
                Price = i.Product.Price            // ✅ កុំអាន i.Price ទៀតទេ
            }).ToList()
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
 
    public async Task<IActionResult> Add(int id, int qty = 1)
    {
        if (qty <= 0) qty = 1;
        var userId = 1; // TODO
        var product = await _db.products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) { TempData["Error"] = "Product not found."; return RedirectToAction("Index", "Menu"); }

        var cart = await _cartService.GetOrCreateAsync(userId);
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            item = new CartItem { CartId = cart.Id, ProductId = id, Quantity = qty ,Price = product.Price/*, Price = product.Price*/ };
            _db.cartItems.Add(item);
        }
        else item.Quantity += qty;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Added to cart.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Remove/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = 1; // TODO
        var item = await _db.cartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.UserId == userId);

        if (item is null)
        {
            TempData["Error"] = "Item not found.";
            return RedirectToAction(nameof(Index));
        }

        _db.cartItems.Remove(item);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Removed.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/ChangeQty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeQty(int id, int qty)
    {
        var userId = 1; // TODO
        if (qty <= 0) qty = 1;

        var item = await _db.cartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.UserId == userId);

        if (item is null)
        {
            TempData["Error"] = "Item not found.";
            return RedirectToAction(nameof(Index));
        }

        item.Quantity = qty;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Quantity updated.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var userId = 1; // TODO
        var cart = await _db.carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null && cart.CartItems.Any())
        {
            _db.cartItems.RemoveRange(cart.CartItems);
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }


}
