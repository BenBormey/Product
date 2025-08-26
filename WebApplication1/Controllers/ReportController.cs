using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class ReportController : Controller
    {
        private readonly AppDbContext _db;
        public ReportController(AppDbContext db) => _db = db;

        // GET: /OrderHistory
        //public async Task<IActionResult> Index(string? status, DateTime? from, DateTime? to, int page = 1, int size = 20)
        //{
        //    var q = _db.orders
        //        .Include(o => o.OrderDetails)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(status))
        //        q = q.Where(o => o.Status == status);

        //    if (from.HasValue)
        //        q = q.Where(o => o.OrderDate >= from.Value.Date);

        //    if (to.HasValue)
        //        q = q.Where(o => o.OrderDate < to.Value.Date.AddDays(1));

        //    var total = await q.CountAsync();

        //    var items = await q
        //        .OrderByDescending(o => o.OrderDate)
        //        .Skip((page - 1) * size)
        //        .Take(size)
        //        .ToListAsync();

        //    ViewBag.Total = total;
        //    ViewBag.Page = page;
        //    ViewBag.Size = size;
        //    ViewBag.Status = status;
        //    ViewBag.From = from;
        //    ViewBag.To = to;

        //    return View(items);
        //}
        //public async Task<IActionResult> Details(int id)
        //{
        //    var order = await _db.orders
        //        .Include(o => o.OrderDetails)
        //        .ThenInclude(i => i.Product) // if you want menu info
        //        .FirstOrDefaultAsync(o => o.Id == id);

        //    if (order == null) return NotFound();

        //    return View(order);
        //}
        //public async Task<IActionResult> ByCustomer()
        //{
        //    return View();  
        //}
        //public async Task<IActionResult> ByProduct()
        //{
        //    return View();
        //}
        //public async Task<IActionResult> ByCategory()
        //{
        //    return View();
        //}
        //public async Task<IActionResult> StockMovement()
        //{
        //    return View();
        //}

  

        [Authorize(Roles = "Admin,Staff")]
        public class ReportsController : Controller
        {
            private readonly AppDbContext _db;
            public ReportsController(AppDbContext db) => _db = db;

            // ================= HUB =================
            [HttpGet]
            public IActionResult Index(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                ViewBag.From = from!.Value.ToString("yyyy-MM-dd");
                ViewBag.To = to!.Value.ToString("yyyy-MM-dd");
                return View();
            }

            // =============== SALES ===============

            // Sales Summary (orders, revenue, avg order value) grouped by period
            //[HttpGet]
            //public async Task<IActionResult> SalesSummary(DateTime? from, DateTime? to, string period = "day")
            //{
            //    (from, to) = NormalizeRange(from, to);

            //    var q = _db.orders.AsNoTracking()
            //        .Where(o => o.Status == "Completed")
            //        .Where(o => o.OrderDate >= from && o.OrderDate < to); // TODO: if you use OrderDate, change here

            //    var data = period.ToLower() switch
            //    {
            //        "week" => await q.GroupBy(o => EF.Functions.DateFromParts(
            //                        o.OrderDate.Value.Year,
            //                        1,
            //                        1).AddDays((int)(Math.Floor(EF.Functions.DateDiffDay(
            //                            new DateTime(o.OrderDate.Value.Year, 1, 1), o.OrderDate) / 7.0) * 7)))
            //                    .Select(g => new { label = g.Key, orders = g.Count(), revenue = g.Sum(x => x.Total), aov = g.Average(x => x.Total) })
            //                    .OrderBy(x => x.label).ToListAsync(),
            //        "month" => await q.GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
            //                    .Select(g => new
            //                    {
            //                        label = new DateTime(g.Key.Year, g.Key.Month, 1),
            //                        orders = g.Count(),
            //                        revenue = g.Sum(x => x.TotalPrice),
            //                        aov = g.Average(x => x.TotalPrice)
            //                    })
            //                    .OrderBy(x => x.label).ToListAsync(),
            //        _ => await q.GroupBy(o => o.OrderDate.Value.Date)
            //                    .Select(g => new { label = g.Key, orders = g.Count(), revenue = g.Sum(x => x.TotalPrice), aov = g.Average(x => x.TotalPrice) })
            //                    .OrderBy(x => x.label).ToListAsync()
            //    };

            //    return Json(data);
            //}

            //[HttpGet]
            //public async Task<IActionResult> ByChannel(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: change o.Channel → your column (e.g., "DINEIN", "TAKEAWAY", "DELIVERY")
            //    var data = await _db.orders.AsNoTracking()
            //        .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
            //        .GroupBy(o => o.Channel) // if no Channel column, remove this report
            //        .Select(g => new { channel = g.Key ?? "Unknown", orders = g.Count(), revenue = g.Sum(x => x.Total) })
            //        .OrderByDescending(x => x.revenue).ToListAsync();
            //    return Json(data);
            //}

            [HttpGet]
            public async Task<IActionResult> ByPayment(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                // TODO: change PaymentMethod to your field name
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
                    .GroupBy(o => o.Status)
                    .Select(g => new { method = g.Key ?? "Unknown", orders = g.Count(), revenue = g.Sum(x => x.TotalPrice) })
                    .OrderByDescending(x => x.revenue).ToListAsync();
                return Json(data);
            }

            [HttpGet]
            public async Task<IActionResult> Hourly(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
                    .GroupBy(o => o.OrderDate.Value.Hour)
                    .Select(g => new { hour = g.Key, orders = g.Count(), revenue = g.Sum(x => x.TotalPrice) })
                    .OrderBy(x => x.hour).ToListAsync();
                return Json(data);
            }

            //[HttpGet]
            //public async Task<IActionResult> Discounts(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: change o.DiscountAmount to your field; else treat as 0
            //    var data = await _db.orders.AsNoTracking()
            //        .Where(o => o.OrderDate >= from && o.OrderDate < to)
            //        .Select(o => new
            //        {
            //            date = o.OrderDate.Value.Date,
            //            discount = (decimal?)(o.DiscountAmount ?? 0m),
            //            total = o.Total
            //        })
            //        .GroupBy(x => x.date)
            //        .Select(g => new { day = g.Key, totalDiscount = g.Sum(x => x.discount), revenue = g.Sum(x => x.total) })
            //        .OrderBy(x => x.day).ToListAsync();
            //    return Json(data);
            //}

            //[HttpGet]
            //public async Task<IActionResult> Cancellations(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: change RefundAmount if exists; else show 0
            //    var data = await _db.orders.AsNoTracking()
            //        .Where(o => o.Status == "CANCELLED" && o.OrderDate >= from && o.OrderDate < to)
            //        .GroupBy(o => o.OrderDate.Value.Date)
            //        .Select(g => new
            //        {
            //            day = g.Key,
            //            cancelled = g.Count(),
            //            amount = g.Sum(x => (decimal?)(x.RefundAmount ?? 0m))
            //        })
            //        .OrderBy(x => x.day).ToListAsync();
            //    return Json(data);
            //}

            [HttpGet]
            public async Task<IActionResult> Daily(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
                    .GroupBy(o => o.OrderDate.Value.Date)
                    .Select(g => new { day = g.Key, orders = g.Count(), quantity = g.SelectMany(x => x.OrderDetails).Sum(i => i.Qty), revenue = g.Sum(x => x.TotalPrice) })
                    .OrderBy(x => x.day).ToListAsync();
                return Json(data);
            }

            // =============== PRODUCT & MENU ===============

            [HttpGet]
            public async Task<IActionResult> ByCustomer(DateTime? from, DateTime? to, int top = 100)
            {
                (from, to) = NormalizeRange(from, to);
                var q = _db.orders.AsNoTracking()
                        .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to);

                var data = await q.GroupBy(o => o.UserId)
                    .Select(g => new
                    {
                        customerId = g.Key,
                        customer = g.Key == null
                            ? "Guest"
                            : _db.users.Where(u => u.Id == g.Key).Select(u => u.Name).FirstOrDefault() ?? "User",
                        orders = g.Count(),
                        quantity = g.SelectMany(x => x.OrderDetails).Sum(i => i.Qty),
                        revenue = g.Sum(x => x.TotalPrice)
                    })
                    .OrderByDescending(x => x.revenue).Take(top).ToListAsync();

                return Json(data);
            }

            [HttpGet]
            public async Task<IActionResult> ByProduct(DateTime? from, DateTime? to, int top = 500)
            {
                (from, to) = NormalizeRange(from, to);
                var q = _db.orderDetail.AsNoTracking()
                    .Include(i => i.Order)
                    .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);

                var data = await q.GroupBy(i => new { i.ProductId })
                    .Select(g => new { productId = g.Key.ProductId, quantity = g.Sum(x => x.Qty), revenue = g.Sum(x => x.Qty * x.Price) })
                    .OrderByDescending(x => x.revenue).Take(top).ToListAsync();

                return Json(data);
            }

            [HttpGet]
            public async Task<IActionResult> ByCategory(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                var q = _db.orderDetail.AsNoTracking()
                    .Include(i => i.Order)
                    .Include(i => i.Product).ThenInclude(m => m.Category)
                    .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);

                var data = await q.GroupBy(i => new { i.Product.CategoryId, Cat = i.Product.Category.CategoryName })
                    .Select(g => new { categoryId = g.Key.CategoryId, category = g.Key.Cat, quantity = g.Sum(x => x.Qty), revenue = g.Sum(x => x.Order.TotalPrice) })
                    .OrderByDescending(x => x.revenue).ToListAsync();

                return Json(data);
            }

            [HttpGet]
            public async Task<IActionResult> TopProducts(DateTime? from, DateTime? to, int top = 20)
            {
                (from, to) = NormalizeRange(from, to);
                var q = _db.orderDetail.AsNoTracking()
                    .Include(i => i.Order)
                    .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);

                var data = await q.GroupBy(i => new { i.ProductId })
                    .Select(g => new { productId = g.Key.ProductId, quantity = g.Sum(x => x.Qty), revenue = g.Sum(x => x.Qty * x.Price) })
                    .OrderByDescending(x => x.revenue).Take(top).ToListAsync();

                return Json(data);
            }

            [HttpGet]
            public async Task<IActionResult> SlowProducts(DateTime? from, DateTime? to, int min = 5)
            {
                (from, to) = NormalizeRange(from, to);
                var q = _db.orderDetail.AsNoTracking()
                    .Include(i => i.Order)
                    .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);

                var data = await q.GroupBy(i => new { i.ProductId })
                    .Select(g => new { productId = g.Key.ProductId, quantity = g.Sum(x => x.Qty), revenue = g.Sum(x => x.Order.TotalPrice) })
                    .Where(x => x.quantity < min)
                    .OrderBy(x => x.quantity).ToListAsync();

                return Json(data);
            }

            //[HttpGet]
            //public async Task<IActionResult> ProductMargin(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: require Menu.UnitCost (decimal). If not in schema, return zeros or add column.
            //    var q = _db.orderDetail.AsNoTracking()
            //        .Include(i => i.Order).Include(i => i.Product)
            //        .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);

            //    var data = await q.GroupBy(i => new { i.ProductId, Name = i.Product.CodeOrBarcode  })
            //        .Select(g => new
            //        {
            //            productId = g.Key.ProductId,
            //            product = g.Key.Name,
            //            quantity = g.Sum(x => x.Qty),
            //            revenue = g.Sum(x => x.Order.TotalPrice),
            //            cogs = g.Sum(x => x.Qty) * (g ?? 0m),
            //            margin = g.Sum(x => x.LineTotal) - (g.Sum(x => x.Qty) * (g.Key.Cost ?? 0m))
            //        })
            //        .OrderByDescending(x => x.margin).ToListAsync();

            //    return Json(data);
            //}

            [HttpGet]
            public async Task<IActionResult> AddOns(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                // TODO: need a flag IsAddon or ParentItemId on OrderItem
                var q = _db.orderDetail.AsNoTracking()
                    .Include(i => i.Order)
                    .Where(i => i.Order.Status == "Completed" && i.Order.OrderDate >= from && i.Order.OrderDate < to);
                  //  .Where(i => i.IsAddon); // change this if your schema differs

                var data = await q.GroupBy(i => i.Product.CodeOrBarcode)
                    .Select(g => new { addon = g.Key, qty = g.Sum(x => x.Qty), revenue = g.Sum(x => x.Order.TotalPrice) })
                    .OrderByDescending(x => x.revenue).ToListAsync();

                return Json(data);
            }

            // =============== INVENTORY ===============

            //[HttpGet]
            //public async Task<IActionResult> StockOnHand()
            //{
            //    var data = await _db.products.AsNoTracking()
            //        .Include(m => m.Category)
            //        .Select(m => new
            //        {
            //            productId = m.Id,
            //            product = m.Name,
            //            category = m.Category != null ? m.Category.CategoryName : "",
            //            onHand = m.QtyInStock,
            //            reorder = m.QtyInStock,
            //            // TODO: if UnitCost exists, include valuation
            //            valuation = (decimal?)((m.Price ?? 0m) * m.Stocks)
            //        })
            //        .OrderBy(x => x.onHand).ToListAsync();
            //    return Json(data);
            //}

            //[HttpGet]
            //public async Task<IActionResult> LowStock()
            //{
            //    var data = await _db.products.AsNoTracking()
            //        .Where(m => m.QtyInStock <= m.ReorderPoint)
            //        .Select(m => new { product = m.Name, onHand = m.StockOnHand, reorder = m.ReorderPoint })
            //        .OrderBy(x => x.onHand).ToListAsync();
            //    return Json(data);
            //}

            //[HttpGet]
            //public async Task<IActionResult> StockInVsOut(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    var q = _db.StockMovements.AsNoTracking().Include(s => s.Menu)
            //        .Where(s => s.OrderDate >= from && s.OrderDate < to);

            //    var data = await q.GroupBy(s => s.Menu.Name)
            //        .Select(g => new
            //        {
            //            product = g.Key,
            //            inQty = g.Where(x => x.Type == "IN").Sum(x => x.Quantity),
            //            outQty = -g.Where(x => x.Type == "OUT").Sum(x => x.Quantity),
            //            net = g.Sum(x => x.Quantity)
            //        })
            //        .OrderByDescending(x => x.net).ToListAsync();

            //    return Json(data);
            //}

            //[HttpGet]
            //public async Task<IActionResult> StockMovement(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    var q = _db.StockMovements.AsNoTracking().Include(s => s.Menu)
            //        .Where(s => s.OrderDate >= from && s.OrderDate < to);

            //    var data = await q.GroupBy(s => new { s.ProductId, s.Menu.Name })
            //        .Select(g => new
            //        {
            //            productId = g.Key.ProductId,
            //            product = g.Key.Name,
            //            inQty = g.Where(x => x.Type == "IN").Sum(x => x.Quantity),
            //            outQty = -g.Where(x => x.Type == "OUT").Sum(x => x.Quantity),
            //            netQty = g.Sum(x => x.Quantity)
            //        })
            //        .OrderByDescending(x => x.netQty).ToListAsync();

            //    return Json(data);
            //}

            // =============== CUSTOMER ===============

            // New vs Returning within range (cohort lite)
            [HttpGet]
            public async Task<IActionResult> CustomerCohorts(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);

                var paid = _db.orders.AsNoTracking().Where(o => o.Status == "Completed");
                // first order date for each customer
                var firsts = await paid
                    .Where(o => o.UserId != null)
                    .GroupBy(o => o.UserId!)
                    .Select(g => new { userId = g.Key, first = g.Min(x => x.OrderDate) })
                    .ToDictionaryAsync(x => x.userId, x => x.first);

                var inRange = await paid
                    .Where(o => o.OrderDate >= from && o.OrderDate < to)
                    .ToListAsync();

                var grouped = inRange.GroupBy(o =>
                {
                    if (o.UserId == null) return "Guest";
                    var first = firsts[o.UserId];
                    return first >= from ? "New" : "Returning";
                })
                .Select(g => new { segment = g.Key, orders = g.Count(), revenue = g.Sum(x => x.TotalPrice) })
                .ToList();

                return Json(grouped);
            }

            // Basic RFM: recency(days since last), frequency(#orders in range), monetary(total in range)
            [HttpGet]
            public async Task<IActionResult> Rfm(DateTime? from, DateTime? to, int top = 200)
            {
                (from, to) = NormalizeRange(from, to);
                var paid = _db.orders.AsNoTracking().Where(o => o.Status == "Completed");

                var lastorders = await paid.GroupBy(o => o.UserId)
                    .Select(g => new { userId = g.Key, last = g.Max(x => x.OrderDate) })
                    .ToListAsync();

                var inRange = await paid.Where(o => o.OrderDate >= from && o.OrderDate < to)
                    .GroupBy(o => o.UserId)
                    .Select(g => new { userId = g.Key, freq = g.Count(), monetary = g.Sum(x => x.TotalPrice) })
                    .ToDictionaryAsync(x => x.userId, x => (x.freq, x.monetary));

                var now = to!.Value;
                var rows = lastorders.Select(lo =>
                {
                    inRange.TryGetValue(lo.userId, out var fm);
                    var recency = (int)(now - lo.last).Value.TotalDays;
                    var customer = lo.userId == null ? "Guest" :
                        _db.users.Where(u => u.Id == lo.userId).Select(u => u.Name).FirstOrDefault() ?? "User";
                    return new { customerId = lo.userId, customer, recency, frequency = fm.freq, monetary = fm.monetary };
                })
                .OrderBy(x => x.recency).ThenByDescending(x => x.monetary).Take(top).ToList();

                return Json(rows);
            }

            // Lifetime value = all-time sum by customer (simple)
            [HttpGet]
            public async Task<IActionResult> Ltv(DateTime? from, DateTime? to, int top = 100)
            {
                // from/to optional; we compute all-time
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.Status == "Completed")
                    .GroupBy(o => o.UserId)
                    .Select(g => new
                    {
                        customerId = g.Key,
                        customer = g.Key == null ? "Guest" :
                            _db.users.Where(u => u.Id == g.Key).Select(u => u.Name).FirstOrDefault() ?? "User",
                        orders = g.Count(),
                        ltv = g.Sum(x => x.TotalPrice)
                    })
                    .OrderByDescending(x => x.ltv).Take(top).ToListAsync();

                return Json(data);
            }

            // =============== OPERATIONS & FINANCE ===============

            [HttpGet]
            public async Task<IActionResult> ByStaff(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                // TODO: change CreatedBy/StaffName to your actual column
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
                    .GroupBy(o => o.User.Name ?? "Unknown")
                    .Select(g => new { staff = g.Key, orders = g.Count(), revenue = g.Sum(x => x.TotalPrice) })
                    .OrderByDescending(x => x.revenue).ToListAsync();
                return Json(data);
            }

            //[HttpGet]
            //public async Task<IActionResult> OperationsSla(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: require timestamps like PreparedAt, ReadyAt, CompletedAt on Order
            //    var q = _db.orders.AsNoTracking()
            //        .Where(o => o.OrderDate >= from && o.OrderDate < to && o.Status == "Completed");

            //    var data = await q.Select(o => new
            //    {
            //        o.Id,
            //        queueMins = (double?)EF.Functions.DateDiffMinute(o.OrderDate, o.PreparedAt ?? o.OrderDate),
            //        prepMins = (double?)EF.Functions.DateDiffMinute(o.PreparedAt ?? o.OrderDate, o.ReadyAt ?? o.OrderDate),
            //        doneMins = (double?)EF.Functions.DateDiffMinute(o.ReadyAt ?? o.OrderDate, o.CompletedAt ?? o.OrderDate)
            //    }).ToListAsync();

            //    var result = new
            //    {
            //        avgQueue = data.Where(x => x.queueMins.HasValue).DefaultIfEmpty().Average(x => x?.queueMins ?? 0),
            //        avgPrep = data.Where(x => x.prepMins.HasValue).DefaultIfEmpty().Average(x => x?.prepMins ?? 0),
            //        avgDone = data.Where(x => x.doneMins.HasValue).DefaultIfEmpty().Average(x => x?.doneMins ?? 0),
            //        samples = data.Count
            //    };

            //    return Json(result);
            //}

            //[HttpGet]
            //public async Task<IActionResult> Voids(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // If you track voided items, change query accordingly.
            //    // Fallback: show stock adjustments tagged CORRECTION in period.
            //    var data = await _db.StockMovements.AsNoTracking()
            //        .Where(s => s.OrderDate >= from && s.OrderDate < to && (s.Reason.Contains("VOID") || s.Reason.Contains("CORRECTION")))
            //        .Select(s => new { date = s.OrderDate, s.ProductId, product = s.Menu.Name, s.Quantity, s.Reason, s.PerformedBy })
            //        .OrderByDescending(x => x.date).ToListAsync();
            //    return Json(data);
            //}

            //[Authorize(Roles = "Admin")]
            //[HttpGet]
            //public async Task<IActionResult> Tax(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    // TODO: change TaxAmount/IsTaxable to your schema
            //    var data = await _db.orders.AsNoTracking()
            //        .Where(o => o.Status == "Completed" && o.OrderDate >= from && o.OrderDate < to)
            //        .GroupBy(o => o.TaxRate) // or single rate -> group by day
            //        .Select(g => new { rate = g.Key, taxable = g.Sum(x => (decimal?)(x.TaxableAmount ?? x.Total)), tax = g.Sum(x => (decimal?)(x.TaxAmount ?? 0m)) })
            //        .ToListAsync();
            //    return Json(data);
            //}

            [Authorize(Roles = "Admin")]
            [HttpGet]
            public async Task<IActionResult> CashDrawer(DateTime? date)
            {
                var day = (date ?? DateTime.Today).Date;
                // If you have CashDrawerLogs, query them. Fallback: summarize orders by payment method for the day.
                var data = await _db.orders.AsNoTracking()
                    .Where(o => o.OrderDate >= day && o.OrderDate < day.AddDays(1) && o.Status == "Completed")
                    .GroupBy(o => o.Status)
                    .Select(g => new { method = g.Key ?? "Unknown", revenue = g.Sum(x => x.TotalPrice) })
                    .OrderByDescending(x => x.revenue).ToListAsync();
                return Json(new { date = day, methods = data });
            }

            //[Authorize(Roles = "Admin")]
            //[HttpGet]
            //public async Task<IActionResult> PnL(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    var revenue = await _db.orders.AsNoTracking()
            //        .Where(o => o.PaymentStatus == "PAID" && o.OrderDate >= from && o.OrderDate < to)
            //        .SumAsync(o => o.Total);

            //    // COGS estimated from UnitCost * Qty (requires UnitCost)
            //    var cogs = await _db.orderDetail.AsNoTracking()
            //        .Include(i => i.Order).Include(i => i.ProductId)
            //        .Where(i => i.Order.PaymentStatus == "PAID" && i.Order.OrderDate >= from && i.Order.OrderDate < to)
            //        .SumAsync(i => (decimal)((i.Menu.UnitCost ?? 0m) * i.Qty));

            //    // TODO: Operating expenses from your Expenses table (if any)
            //    decimal opex = 0m;

            //    return Json(new { revenue, cogs, gross = revenue - cogs, opex, net = revenue - cogs - opex });
            //}

            //[HttpGet]
            //public async Task<IActionResult> Campaigns(DateTime? from, DateTime? to)
            //{
            //    (from, to) = NormalizeRange(from, to);
            //    //o.PaymentStatus == "PAID" &&&& o.con != null
            //    // TODO: change CouponCode/PromotionId to your schema
            //    var data = await _db.orders.AsNoTracking()
            //        .Where(o =>  o.OrderDate >= from && o.OrderDate < to )
            //        .GroupBy(o => o.CouponCode)
            //        .Select(g => new { code = g.Key!, uses = g.Count(), revenue = g.Sum(x => x.Total), discount = g.Sum(x => (decimal?)(x.DiscountAmount ?? 0m)) })
            //        .OrderByDescending(x => x.revenue).ToListAsync();
            //    return Json(data);
            //}

            [HttpGet]
            public async Task<IActionResult> Feedback(DateTime? from, DateTime? to)
            {
                (from, to) = NormalizeRange(from, to);
                // TODO: if you have a Feedbacks table, join it; else return empty
                var data = await _db.reviews.AsNoTracking()
                    .Where(f => f.ReviewDate >= from && f.ReviewDate < to)
                    .Select(f => new { date = f.ReviewDate, rating = f.Rating, comment = f.Comment, customer = f.User.Name })
                    .OrderByDescending(x => x.date).ToListAsync();
                return Json(data);
            }

            // ================= Helpers =================
            private static (DateTime?, DateTime?) NormalizeRange(DateTime? from, DateTime? to)
            {
                var f = (from ?? DateTime.Today.AddDays(-30)).Date;
                var t = (to ?? DateTime.Today).Date.AddDays(1); // exclusive
                return (f, t);
            }
        }
    }

}

