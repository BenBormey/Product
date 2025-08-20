//using Microsoft.EntityFrameworkCore;
//using WebApplication1.Data;
//using WebApplication1.Models;

//public class AnalyticsEfService
//{
//    private readonly AppDbContext _db;
//    public AnalyticsEfService(AppDbContext db) => _db = db;

//    private static bool IsActiveStatus(string? s)
//        => string.IsNullOrEmpty(s) || !string.Equals(s, "CANCELLED", StringComparison.OrdinalIgnoreCase);

//    // 🔹 KPIs: Orders Count, Revenue, Avg Order Value
//    public async Task<KpiDTO> GetKpisAsync(DateTime from, DateTime to, CancellationToken ct = default)
//    {
//        var orderTotals = await _db.orders
//            .AsNoTracking()
//            .Where(o => o.OrderDate != null
//                     && o.OrderDate >= from && o.OrderDate < to
//                     && (o.Status == null || o.Status != "CANCELLED"))
//            .Select(o => new
//            {
//                o.Id,
//                Total = o.OrderDetails.Sum(od => od.Qty * od.Price)
//            })
//            .ToListAsync(ct);

//        var ordersCount = orderTotals.Count;
//        var revenue = orderTotals.Sum(x => x.Total);
//        var aov = ordersCount == 0 ? 0m : orderTotals.Average(x => x.Total);
//        return new KpiDTO(ordersCount, revenue, aov);
//    }


//    // 🔹 Daily sales (time series)



//    //public async Task<List<DailyPointDTO>> GetDailyAsync(DateTime from, DateTime to, CancellationToken ct = default)
//    //{
//    //    return await _db.orderDetail
//    //        .Where(od => od.Order != null
//    //                  && od.Order.OrderDate >= from
//    //                  && od.Order.OrderDate < to
//    //                  && (od.Order.Status == null || od.Order.Status != "CANCELLED"))
//    //        .GroupBy(od => new {
//    //            od.Order!.OrderDate.Value.Year,
//    //            od.Order!.OrderDate.Value.Month,
//    //            od.Order!.OrderDate.Value.Day
//    //        })
//    //        .Select(g => new DailyPointDTO(
//    //            new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
//    //            g.Sum(od => (decimal)od.Qty * od.Price)     // 👈 no AsQueryable()
//    //        ))
//    //        .OrderBy(x => x.Date)
//    //        .ToListAsync(ct);
//    //}



//    //// 🔹 Top Products by Revenue
//    //public async Task<List<TopProductDTO>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10, CancellationToken ct = default)
//    //{
//    //    return await _db.orderDetail
//    //        .AsNoTracking()
//    //        .Where(od => od.Order.OrderDate != null
//    //                  && od.Order.OrderDate >= from && od.Order.OrderDate < to
//    //                  && (od.Order.Status == null || od.Order.Status != "CANCELLED"))
//    //        .GroupBy(od => new { od.ProductId, od.Product.Name })
//    //        .Select(g => new TopProductDTO(
//    //            g.Key.ProductId,
//    //            g.Key.Name,
//    //            g.Sum(x => x.Qty),
//    //            g.Sum(x => x.Qty * x.Price)
//    //        ))
//    //        .OrderByDescending(x => x.TotalRevenue)
//    //        .Take(top)
//    //        .ToListAsync(ct);
//    //}


//    //// 🔹 Revenue by Category
//    //public async Task<List<CategoryRevenueDTO>> GetByCategoryAsync(DateTime from, DateTime to, CancellationToken ct = default)
//    //{
//    //    return await _db.orderDetail
//    //        .AsNoTracking()
//    //        .Where(od => od.Order.OrderDate != null
//    //                  && od.Order.OrderDate >= from && od.Order.OrderDate < to
//    //                  && (od.Order.Status == null || od.Order.Status != "CANCELLED"))
//    //        .GroupBy(od => new { od.Product.CategoryId, od.Product.Category.CategoryName })
//    //        .Select(g => new CategoryRevenueDTO(
//    //            g.Key.CategoryId,
//    //            g.Key.CategoryName!,
//    //            g.Sum(x => x.Qty * x.Price)
//    //        ))
//    //        .OrderByDescending(x => x.Revenue)
//    //        .ToListAsync(ct);
//    //}

//}
