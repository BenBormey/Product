using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebApplication1.Data;
using WebApplication1.Models.Dashboard;
using WebApplication1.Repository;

namespace WebApplication1.service
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;
        public DashboardService(AppDbContext contex)
        {
            _db = contex;
            
        }
        public async Task<DashboardCardsDto> GetCardsAsync(DateTime today, int days = 30)
        {
            var start = today.Date.AddDays(-(days -1 ));


            //rok comnul for month
            var revenueToday = await _db.orders
                .Where(o => o.Status == "Completed" && o.OrderDate.Value.Date == today.Date)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

            //  Rok count del ben order
            var ordersToday = await _db.orders
                .CountAsync(o => o.OrderDate.Value.Date == today.Date);

            // last date
            var revenueLastDays = await _db.orders
                .Where(o => o.Status == "Completed" && o.OrderDate >= start && o.OrderDate < today.AddDays(1))
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;


       //     var expensesLastDays = await _db.Set<Expense>()
       //.Where(e => e.Date >= start && e.Date < today.AddDays(1))
       //.SumAsync(e => (decimal?)e.Amount) ?? 0m;

            var avgRevenue = days == 0 ? 0 : revenueLastDays / days;
          //  var avgExpense = days == 0 ? 0 : expensesLastDays / days;

            return new DashboardCardsDto(revenueToday, ordersToday, avgRevenue);

        }

        public async Task<SeriesDto> GetOrdersPerDayAsync(DateTime endInclusive, int days = 7)
        {
            var start = endInclusive.Date.AddDays(-(days -1 ));

            var raw = await _db.orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= endInclusive)
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(o => new { Day = o.Key, Cnt = o.Count() })
                .ToListAsync();


            var labels = Enumerable.Range(0, days)
                .Select(i => start.AddDays(i))
                .Select(d => d.ToString("ddd"))
                .ToList();




            var data = Enumerable.Range(0, days)
              .Select(i => start.AddDays(i))
              .Select(day => (decimal)(raw.FirstOrDefault(r => r.Day == day)?.Cnt ?? 0)) // <-- add ?? 0
              .ToList();


            return new SeriesDto(labels, data);

        }

        public async Task<DoughnutDto> GetOrderStatusDoughnutAsync(DateTime month)
        {
            var m0 = new DateTime(month.Year, month.Month, 1);
            var m1 = m0.AddMonths(1);

            var q = await _db.orders
                .Where(o => o.OrderDate >= m0 && o.OrderDate < m1)
                .GroupBy(o => o.Status)
                .Select(o => new { o.Key, Cnt = o.Count() })
                .ToListAsync();

            var slices = new List<DoughnutSliceDto>
            {
                    new("Completed", q.FirstOrDefault(x => x.Key == "Completed")?.Cnt ?? 0),
            new("Processing", q.FirstOrDefault(x => x.Key == "Processing")?.Cnt ?? 0),
            new("Cancelled",  q.FirstOrDefault(x => x.Key == "Cancelled")?.Cnt ?? 0),
            new("Refunded",   q.FirstOrDefault(x => x.Key == "Refunded")?.Cnt ?? 0)
         
            };
            return new DoughnutDto(slices);
        }

        public async Task<List<TrendingItemDto>> GetTrendingAsync(DateTime day, int top = 3)
        {
            var q = await _db.orderDetail
                .Where(oi => oi.Order!.OrderDate.Value.Date == day.Date && oi.Order.Status == "PENDING")
                .GroupBy(oi =>oi.ProductId)
                .Select(g => new { ProductId = g.Key, Qty = g.Sum(x =>x.Qty), Revenue = g.Sum(x => x.Qty * x.Price) })
                .OrderByDescending(x => x.Revenue)
                .Take(top).
                Join(_db.products, x =>x.ProductId ,p => p.Id,
                (x,p) => new TrendingItemDto(p.Id,p.Name,p.Price,p.Image))
                .ToListAsync();
            return q;

        }
    }
}
