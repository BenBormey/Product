using WebApplication1.Models.Dashboard;

namespace WebApplication1.Repository
{
    public interface IDashboardService
    {
        Task<DashboardCardsDto> GetCardsAsync(DateTime today, int days = 30);
        Task<DoughnutDto> GetOrderStatusDoughnutAsync(DateTime month);
        Task<SeriesDto> GetOrdersPerDayAsync(DateTime endInclusive, int days = 7);
        Task<List<TrendingItemDto>> GetTrendingAsync(DateTime day, int top = 3);
    }
}
