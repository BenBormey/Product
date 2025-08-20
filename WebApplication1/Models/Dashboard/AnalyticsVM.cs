namespace WebApplication1.Models.Dashboard
{
    public class AnalyticsVM
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // Cards, Doughnut, Series, Trending (from your DTOs)
        public DashboardCardsDto Cards { get; set; } = new();
        public DoughnutDto OrderStatus { get; set; } = new();
        public SeriesDto OrdersPerDay { get; set; } = new();
        public List<TrendingItemDto> Trending { get; set; } = new();

        // Optional: compatibility mapper if your view uses Model.Kpis.*
        public CardsLegacy Kpis => new()
        {
            RevenueToday = Cards.TodaysRevenue,
            OrdersToday = Cards.TodaysOrder,
            AvgOrderValue = Cards.AvgRevenue
        };

        public class CardsLegacy
        {
            public decimal RevenueToday { get; set; }
            public int OrdersToday { get; set; }
       
            public decimal AvgOrderValue { get; set; }
        }
    }
}
