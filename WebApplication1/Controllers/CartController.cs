using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Models.Invoice;
using WebApplication1.Models.Order; // ប្តូរប្រសិនបើ namespace ផ្សេង
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


        var now = DateTime.UtcNow;

        var prodis = await _db.promotions
            .Where(pr => pr.ProductId == id
                && (pr.StartDate == null || pr.StartDate <= now)
                && (pr.EndDate == null || now <= pr.EndDate))
            .OrderByDescending(pr => (double)pr.DiscountPercent)                 
            .ThenBy(pr => pr.EndDate )        
            .FirstOrDefaultAsync();


        if (product is null) { TempData["Error"] = "Product not found."; return RedirectToAction("Index", "Menu"); }

        var cart = await _cartService.GetOrCreateAsync(userId);
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            item = new CartItem { CartId = cart.Id, ProductId = id, Quantity = qty, Price = product.Price,dis = Math.Round(product.Price * (1 - ((decimal)prodis.DiscountPercent / 100m)), 2) };
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
    [HttpGet]
    public async Task<IActionResult> Details(int id, decimal CashAmount)
    {
        var order = await _db.orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        // Load items via OrderDetail table (robust even if Order has no nav collection)
        var items = await _db.orderDetail
            .Where(od => od.OrderId == id)
            .Include(od => od.Product)
            .Select(od => new OrderItemRow
            {
                ProductId = od.ProductId,
                Name = od.Product != null ? od.Product.Name : $"#{od.ProductId}",
                ImageUrl = od.Product != null ? od.Product.Image : null,
                UnitPrice = od.Price,
                Qty = od.Qty,
                LineTotal = od.Price * od.Qty
            })
            .ToListAsync();
        order.cashAmount = (decimal)CashAmount;
        order.cashreturn = (decimal)(CashAmount - order.TotalPrice);
        await _db.SaveChangesAsync();

        var vm = new OrderDetailsVm
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            Items = items,
            cashreturn = (decimal)order.cashreturn,

        };

        return View(vm); // Views/Orders/Details.cshtml
    }
    public async Task<IActionResult> Invoice(int Id, bool receipt = false, CancellationToken ct = default)
    {
        var order = await _db.orders.FirstOrDefaultAsync(o => o.Id == Id, ct);
        var user = await _db.users.FirstOrDefaultAsync();
        if (order is null) return NotFound();

        // JOIN order_detail → products
        var items = await _db.orderDetail
            .Where(d => d.OrderId == Id)
            .Join(_db.products,
                  d => d.ProductId,             // <-- change if your FK has another name
                  p => p.Id,
                  (d, p) => new PrintItemVM
                  {
                      Code = p.CodeOrBarcode, // from products
                      Name = p.Name,
                      Qty = d.Qty,      // <-- CHANGE HERE if your column is "Qty"
                      UnitPrice = d.Price == 0 ? p.Price : d.Price,  // fallback to product price
                      LineTotal = d.Qty * (d.Price == 0 ? p.Price : d.Price)
                  })
            .ToListAsync(ct);

        var subtotal = items.Sum(x => x.LineTotal);
        var grandTotal = subtotal;              // no VAT/fees/discounts in minimal schema

        var paid =order.cashAmount ?? 0m;                        // set if you store a paid amount
        var chg = order.cashreturn ?? 0m;             // you can pass ?change= from COD
        var due = Math.Max(grandTotal - paid, 0m);
        var phone = user?.Phone;

        order.Status = "Completed";
        await _db.SaveChangesAsync();

        var vm = new PrintOrderVM
        {
            OrderId = order.Id,
            // order.OrderDate is likely DateTime? from DB "date"
            OrderDate = (DateTime)(order.OrderDate == default ? DateTime.Now : order.OrderDate),
            Items = items,
            Subtotal = subtotal,
            GrandTotal = grandTotal,
            Paid = paid,
            Change = (decimal)chg,
            Due = due,
            IsReceipt = receipt,
            Phone = phone,
        };
   

        return View("Invoice", vm);
    }
    public async Task<IActionResult> Pedding()
    {
        var result = await _db.orders
    .AsNoTracking()
    .Where(p => p.Status == "Pending" || p.Status == "Peding")
    .ToListAsync();

        return View(result);
        
    }
    public async Task<IActionResult> RemoveOrder(int orderId, CancellationToken ct)
    {
        var order = await _db.orders.FirstOrDefaultAsync(x => x.Id == orderId, ct);
        if (order == null) return NotFound();

        // បិទ order ដោយកែស្ថានភាព
        if (!string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            order.Status = "Cancelled";
            await _db.SaveChangesAsync(ct);
        }

        return RedirectToAction("Pedding", "Cart");
    }
    public async Task<IActionResult> DetailsOrder(int id, CancellationToken ct)
    {
        var order = await _db.orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)  // only if you have navigation Product
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }


}
