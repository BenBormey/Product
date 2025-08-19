
namespace WebApplication1.Models
{

    public record KpiDTO(int OrdersCount, decimal Revenue, decimal AvgOrderValue);
    public record DailyPointDTO(DateTime Date, decimal Revenue);
    public record TopProductDTO(int Id, string Name, int TotalQty, decimal TotalRevenue);
    public record CategoryRevenueDTO(int Id, string CategoryName, decimal Revenue);

    public class AnalyticsVM
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public KpiDTO Kpis { get; set; } = new(0, 0, 0);
        public List<DailyPointDTO> Daily { get; set; } = new();
        public List<TopProductDTO> TopProducts { get; set; } = new();
        public List<CategoryRevenueDTO> ByCategory { get; set; } = new();
    }

}